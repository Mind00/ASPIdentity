using AspCoreIdentity.Models;
using AspCoreIdentity.Models.incoming;
using AspCoreIdentity.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Services
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> ListAsync();
        Task<ApplicationUser> GetUserById(string id);
        Task<UserManagerResponse<ApplicationUser>> CreateUserAsync(ApplicationUser model);
        Task<UserManagerResponse<ApplicationUser>> UpdateUserAsync(ApplicationUser model);
        Task<UserManagerResponse<ApplicationUser>> DeleteUserAsync(string id);
        Task<UserManagerResponse<ApplicationRoles>> AddUserToRole(string email, string roleName);
        Task<UserManagerResponse<ApplicationUser>> RemoveUserFromRole(string email, string roleName);
        Task<UserLoginResponse> Login(ApplicationUser user);
        Task<RefreshToken> GetByRefreshToken(string refreshToken);
        Task<AuthResult> VerifyToken(TokenRequest tokenRequest);
        Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken);
    }
}
