using AspCoreIdentity.Configuration;
using AspCoreIdentity.Models;
using AspCoreIdentity.Models.incoming;
using AspCoreIdentity.Responses;
using AspCoreIdentity.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspCoreIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly JwtConfig _jwtConfig;

        public AccountsController(IUserService userService, 
            IMapper mapper, 
            IOptionsMonitor<JwtConfig> _optionMonitor
            )
        {
            _userService = userService;
            _mapper = mapper;
            _jwtConfig = _optionMonitor.CurrentValue;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetAllUserAsync()
        {
            var users = await _userService.ListAsync();
            var dto = _mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserDto>>(users);

            return dto;

        }


        // GET api/<AccountsController>/5
        [HttpGet("{id}")]
        public async Task<UserDto> Get(string id)
        {
            var user = await _userService.GetUserById(id);
            var dto = _mapper.Map<ApplicationUser, UserDto>(user);
            return dto;
        }

        // POST api/<AccountsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] registerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<registerViewModel, ApplicationUser>(model);
                var registeredUser = await _userService.CreateUserAsync(dto);
                if (registeredUser.isSuccess)
                {
                    return Ok(registeredUser.Message);
                }
                else
                {
                    return BadRequest(registeredUser.Errors);
                }
            }
            else
            {
                return BadRequest("some validation errors.");
            }
        }

        // PUT api/<AccountsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put( [FromBody] registerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<registerViewModel, ApplicationUser>(model);
                var updateUser = await _userService.UpdateUserAsync(dto);
                if (updateUser.isSuccess)
                {
                    return Ok(updateUser.Message);
                }
                else
                {
                    return BadRequest(updateUser.Errors);
                }
            }
            else
            {
                return BadRequest("Some Validation error occur.");
            }
        }

        // DELETE api/<AccountsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deletedUser = await _userService.DeleteUserAsync(id);
            if (deletedUser.isSuccess)
            {
                return Ok(deletedUser.Message);
            }
            else
            {
                return BadRequest(deletedUser.Errors);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<UserLoginRequest, ApplicationUser>(loginRequest);
                var result = await _userService.Login(dto);
                if (result.success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            else
            {
                return BadRequest(new UserLoginResponse()
                {
                    success = false,
                    Errors = new List<string>()
                    {
                        "Some Error Occured."
                    }
                });
            }
        }

        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
                //check if the token is valid
                var result = await _userService.VerifyToken(tokenRequest);
                if(result == null)
                {
                    //return BadRequest(new UserManagerResponse<TokenRequest>
                    //{
                    //    isSuccess = false,
                    //    Errors = "Token validation false"
                    //});
                    return BadRequest(new AuthResult()
                    {
                        success = false,
                        Errors = new List<string>()
                        {
                            "Token Validation false"
                        }
                    }); ;
                }
                return Ok(result); 
            }
            else
            {
                return BadRequest(new UserManagerResponse<TokenRequest>
                {
                    isSuccess = false,
                    Errors = "Invalid payload"
                });
            }
        }

        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var result = await _userService.AddUserToRole(email, roleName);
            if (result.isSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("RemoveRoles")]
        public async Task<IActionResult> RemoveRoleFromUser(string email, string roleName)
        {
            var result = await _userService.RemoveUserFromRole(email, roleName);
            if (result.isSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Errors);
        }
    }
}
