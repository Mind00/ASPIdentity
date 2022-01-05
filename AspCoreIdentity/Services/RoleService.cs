using AspCoreIdentity.Models;
using AspCoreIdentity.Responses;
using AspCoreIdentity.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspCoreIdentity.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRoles> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RoleService> _logger;

        public RoleService(RoleManager<ApplicationRoles> roleManager, ILogger<RoleService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<RoleResponse<ApplicationRoles>> CreateRoles(ApplicationRoles roles)
        {
            try
            {
                var existingRole = await _roleManager.RoleExistsAsync(roles.Name);
                if (!existingRole)
                {
                    var newRole = new ApplicationRoles
                    {
                        Name = roles.Name,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier).ToString()
                    };
                    await _roleManager.CreateAsync(newRole);
                    return new RoleResponse<ApplicationRoles>
                    {
                        isSuccess = true,
                        Message = "Role added successfully."
                    };
                }
                else
                {
                    return new RoleResponse<ApplicationRoles>
                    {
                        isSuccess = false,
                        Message = "This Role aready exist"
                    };
                }
            }
            catch(Exception ex)
            {
                return new RoleResponse<ApplicationRoles>
                {
                    isSuccess = false,
                    Errors = ex.Message
                };
            }
        }

        public async Task<RoleResponse<ApplicationRoles>> DeleteRoles(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new RoleResponse<ApplicationRoles>
                {
                    isSuccess = false,
                    Message = "Record Not found"
                };
            }
            else
            {
                try
                {
                    await _roleManager.DeleteAsync(role);
                    return new RoleResponse<ApplicationRoles>
                    {
                        isSuccess = true,
                        Message = "Record deleted successfully"
                    };
                }catch(Exception ex)
                {
                    return new RoleResponse<ApplicationRoles>
                    {
                        isSuccess = false,
                        Errors = ex.Message
                    };
                }
            }
        }

        public async Task<RoleResponse<ApplicationRoles>> GetRoleById(string id)
        {
            try
            {
                var record = await _roleManager.FindByIdAsync(id);
                if (record == null)
                {
                    return new RoleResponse<ApplicationRoles>
                    {
                        isSuccess = false,
                        Message = "No Record Found."
                    };
                }
                else
                {
                    return new RoleResponse<ApplicationRoles>
                    {
                        isSuccess = true,
                        Data = record
                    };
                }
            }catch(Exception ex)
            {
                return new RoleResponse<ApplicationRoles>
                {
                    isSuccess = false,
                    Errors = ex.Message
                };
            }
        }

        public async Task<IEnumerable<ApplicationRoles>> ListAsync()
        {
            try
            {
                var records = await _roleManager.Roles.AsNoTracking().ToListAsync();
                return records; 
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoleResponse<ApplicationRoles>> UpdateRoles(string id, ApplicationRoles roles)
        {
            var existingRole = await _roleManager.FindByIdAsync(id);
            try
            {
                if (existingRole != null)
                {
                    existingRole.Name = roles.Name;
                    existingRole.CreatedAt = DateTime.UtcNow;
                    existingRole.CreatedBy = "1";
                    await _roleManager.UpdateAsync(existingRole);
                    return new RoleResponse<ApplicationRoles>()
                    {
                        isSuccess = true,
                        Message = "Record Updated Successfully."
                    };
                }
                else
                {
                    return new RoleResponse<ApplicationRoles>
                    {
                        isSuccess = false,
                        Message = "Record Not Found."
                    };
                }
            }catch(Exception ex)
            {
                return new RoleResponse<ApplicationRoles>()
                {
                    isSuccess = false,
                    Errors = ex.Message
                };
            }
        }
    }
}
