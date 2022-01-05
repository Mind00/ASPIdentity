using AspCoreIdentity.Configuration;
using AspCoreIdentity.Context;
using AspCoreIdentity.Models;
using AspCoreIdentity.Models.incoming;
using AspCoreIdentity.Models.outgoing;
using AspCoreIdentity.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspCoreIdentity.Services
{
    public class UserService : IUserService
    {
        public UserManager<ApplicationUser> _userManager;
        public RoleManager<ApplicationRoles> _roleManager;
        private readonly TokenValidationParameters _tokenValidationsParameters;
        private readonly JwtConfig _jwtConfig;
        private readonly AppDbContext _context;
        public UserService(UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRoles> roleManager,
            IOptionsMonitor<JwtConfig> _optionMonitor,
            TokenValidationParameters tokenValidationParameters,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtConfig = _optionMonitor.CurrentValue;
            _tokenValidationsParameters = tokenValidationParameters;
            _context = context;
        }
        public async Task<UserManagerResponse<ApplicationUser>> CreateUserAsync([FromBody]ApplicationUser model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if(existingUser != null)
            {
                return new UserManagerResponse<ApplicationUser>
                {
                    isSuccess = false,
                    Message = " Email already exist. Please try another."
                };
            }
            else
            {
                var newUser = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                };
                try
                {
                    await _userManager.CreateAsync(newUser, model.PasswordHash);
                    return new UserManagerResponse<ApplicationUser>
                    {
                        isSuccess = true,
                        Message = "User Added successfully."

                    };
                }
                catch(Exception ex)
                {
                    return new UserManagerResponse<ApplicationUser>
                    {
                        isSuccess = false,
                        Errors = ex.Message
                    };
                }
                
            }
        }

        public async Task<UserManagerResponse<ApplicationUser>> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            try
            {
                if (user == null)
                {
                    return new UserManagerResponse<ApplicationUser>
                    {
                        isSuccess = false,
                        Message = "No record found."
                    };
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                    return new UserManagerResponse<ApplicationUser>
                    {
                        isSuccess = true,
                        Message = "Record deleted successfully."
                    };
                }
            }catch(Exception ex)
            {
                return new UserManagerResponse<ApplicationUser>
                {
                    isSuccess = false,
                    Errors = ex.Message
                };
            }
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return user;
        }

        public async Task<IEnumerable<ApplicationUser>> ListAsync()
        {
            var userList = await _userManager.Users.AsNoTracking().ToListAsync();
            return (userList);
        }

        public async Task<UserManagerResponse<ApplicationUser>> UpdateUserAsync(ApplicationUser model)
        {
            var existingUser = await _userManager.FindByIdAsync(model.Id);
            try
            {
                if (existingUser == null)
                {
                    return new UserManagerResponse<ApplicationUser>
                    {
                        isSuccess = false,
                        Message = "No record found."
                    };
                }
                else
                {
                    var emailExist = await _userManager.FindByEmailAsync(model.Email);
                    if (emailExist == null)
                    {
                        return new UserManagerResponse<ApplicationUser>
                        {
                            isSuccess = false,
                            Message = "Email already in User.Please try another."
                        };
                    }
                    else
                    {
                        existingUser.FullName = model.FullName;
                        existingUser.Email = model.Email;
                        existingUser.UserName = model.Email;
                        await _userManager.UpdateAsync(existingUser);
                        return new UserManagerResponse<ApplicationUser>
                        {
                            isSuccess = true,
                            Message = "Record updated successfully."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new UserManagerResponse<ApplicationUser>
                {
                    isSuccess = false,
                    Errors = ex.Message
                };
            }
        }

        public async Task<UserLoginResponse> Login(ApplicationUser user)
        {
            try
            {
                var emailExist = await _userManager.FindByEmailAsync(user.Email);
                if (emailExist != null && await _userManager.CheckPasswordAsync(emailExist, user.PasswordHash))
                {
                    var jwtToken = await GenerateToken(emailExist);
                    return new UserLoginResponse
                    {
                        success = true,
                        token = jwtToken.jwtToken,
                        refreshToken = jwtToken.refreshToken
                    };
                }
                else
                {
                    return new UserLoginResponse
                    {
                        success = false,
                        Errors = new List<string>
                    {
                        "Email or Password do not match."
                    }
                    };
                }
            }
            catch (Exception ex)
            {
                return new UserLoginResponse
                {
                    Errors = new List<string>
                    {
                        ex.Message
                    }
                };
            }
        }

        private async Task<TokenResource> GenerateToken(ApplicationUser user)
        {
            //handler responsible for creating the token
            var jwtHandler = new JwtSecurityTokenHandler();
            //get security key
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var claims = await GetAllValidClaims(user);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            //generate the security obj token
            var token = jwtHandler.CreateToken(tokenDescriptor);

            //convert the security obj token into a string
            var jwtToken = jwtHandler.WriteToken(token);

            //generate refresh token
            var refreshToken = new RefreshToken
            {
                AddedDated = DateTime.UtcNow,
                Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
                UserId = user.Id,
                IsRevoked = false,
                IsUserd = false,
                JwtId = token.Id,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };
            
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            var tokenData = new TokenResource
            {
                jwtToken = jwtToken,
                refreshToken = refreshToken.Token
            };

            return tokenData ;
        }

        //method to get all valid claims for the corresponding user
        private async Task<List<Claim>> GetAllValidClaims(ApplicationUser user)
        {
            var _options = new IdentityOptions();
            var claims = new List<Claim>
            {
                    new Claim("Id", user.Id),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  //used by refresh token
            };
            //Getting the claims that we have assigned to the user
            var userClaim = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaim);

            //Get the role and add it to the claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach(var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach(var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

        // method to generate random string 
        private string RandomStringGenerator(int length)
        {
            var random = new Random();
            const string chars = "ienlsidf1243jlkdjfj00998444@@@@*jfiro4287AAANNNIIJKmdfjss";
            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
        }

        public async Task<AuthResult> VerifyToken(TokenRequest tokenRequest)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // we need to check the validity of the token
                var principal = tokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationsParameters, out var validatedToken);
                //we need to validate the results that has been generated
                //Validate if the string is an acutal JWT token not a random string
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    //check if the token is created with the same algorithm as our jwt token
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                        return null;
                }
                // we need to check the expiry date of the token
                var expTime = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                //convert to expTime to date format to check
                var expDate = UnixTimeStampToDateTime(expTime);
                //checking if the jwt token has been expired??
                if (expDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        success = false,
                        Errors = new List<string>
                        {
                            "Token has not expired"
                        }
                    };
                }
                //checking if the refresh token exist
                var refreshTokenExist = await GetByRefreshToken(tokenRequest.RefreshToken);
                if(refreshTokenExist == null)
                {
                    return new AuthResult()
                    {
                        success = false,
                        Errors = new List<string>()
                        {
                            "Invalid Token"
                        }
                    };
                }
                //check the expiry date of a refresh token
                if(refreshTokenExist.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        success = false,
                        Errors = new List<string>()
                        {
                            "Token Expired. Login Again.."
                        }
                    };
                }
                //check if refresh token has been used or not
                if (refreshTokenExist.IsUserd)
                {
                    return new AuthResult()
                    {
                        success = false,
                        Errors = new List<string>()
                        {
                            "Refresh token has been used.It cannot be reused."
                        }
                    };
                }
                //check if it has been revoked
                if (refreshTokenExist.IsRevoked)
                {
                    return new AuthResult()
                    {
                        success = false,
                        Errors = new List<string>()
                        {
                            "Refresh token has been revoked.It cannot be reused."
                        }
                    };
                }

                var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if(refreshTokenExist.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        success = false,
                        Errors = new List<string>()
                        {
                            "Refresh token does been matched with jwt token."
                        }
                    };
                }

                //start processing for getting a new token
                refreshTokenExist.IsUserd = true;
                // Call MarkRefreshTokenAsUsed
                var updateResult = await MarkRefreshTokenAsUsed(refreshTokenExist);
                if (updateResult)
                {
                   await _context.SaveChangesAsync();
                    //get a user generate a new jwt token
                    var dbUser = await _userManager.FindByIdAsync(refreshTokenExist.UserId);
                    if (dbUser == null)
                    {
                        return new AuthResult()
                        {
                            success = false,
                            Errors = new List<string>()
                            {
                                "Error processing request"
                            }
                        };
                    }

                    //generate token
                    var tokens = await GenerateToken(dbUser);
                    return new AuthResult()
                    {
                        success = true,
                        token = tokens.jwtToken,
                        refreshToken = tokens.refreshToken
                    };
                }
                return new AuthResult()
                {
                    success = false,
                    Errors = new List<string>()
                    {
                        "Error processing reques"
                    }
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        } 

        

        private DateTime UnixTimeStampToDateTime(long unixDate)
        {
            //sets the time to 1, Jan 1970
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            //add the number of seconds from 1 Jan 1970
            dateTime = dateTime.AddSeconds(unixDate).ToUniversalTime();
            return dateTime;
        }

        public async Task<RefreshToken> GetByRefreshToken(string refreshToken)
        {
            try
            {
                return await _context.RefreshTokens.Where(x => x.Token.ToLower() == refreshToken.ToLower())
                    .AsNoTracking().FirstOrDefaultAsync();
            }catch(Exception)
            {
                return null;
            }
        }

        public async Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken)
        {
            try
            {
                var token = await _context.RefreshTokens.Where(x => x.Token.ToLower() == refreshToken.Token.ToLower()).AsNoTracking().FirstOrDefaultAsync();
                if (token == null)
                    return false;
                token.IsUserd = refreshToken.IsUserd;
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            
        }

        public async Task<UserManagerResponse<ApplicationRoles>> AddUserToRole(string email, string roleName)
        {
            //check if the user exist
            var userExist = await _userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                //_logger.LogInformation($"The user with this {email} does not exist");
                return new UserManagerResponse<ApplicationRoles>()
                {
                    isSuccess = false,
                    Errors =  "Email does not exist."
                };
            }
            else
            {
                //check if the role exist
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //_logger.LogInformation($"The role {roleName} does not exist.");
                    return new UserManagerResponse<ApplicationRoles>()
                    {
                        isSuccess = false,
                        Errors =  "The role does not exist."
                    };
                }
                else
                {
                    var result = await _userManager.AddToRoleAsync(userExist, roleName);
                    return new UserManagerResponse<ApplicationRoles>()
                    {
                        isSuccess = true,
                        Message = "Role is assigned to the user."
                    };
                }
            }
        }
        public async Task<UserManagerResponse<ApplicationUser>> RemoveUserFromRole(string email, string roleName)
        {
            //check if the user exist
            var userExist = await _userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                //_logger.LogInformation($"The user with this {email} does not exist");
                return new UserManagerResponse<ApplicationUser>()
                {
                    isSuccess = false,
                    Errors = "Email does not exist."
                };
            }
            else
            {
                //check if the role exist
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //_logger.LogInformation($"The role {roleName} does not exist.");
                    return new UserManagerResponse<ApplicationUser>()
                    {
                        isSuccess = false,
                        Errors = "The role does not exist."
                    };
                }
                else
                {
                    var result = await _userManager.RemoveFromRoleAsync(userExist, roleName);
                    return new UserManagerResponse<ApplicationUser>()
                    {
                        isSuccess = true,
                        Message = $"The {roleName} is removed from the User{email}"
                    };
                }
            }
        }
    }
}
