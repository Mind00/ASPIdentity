using AspCoreIdentity.Models;
using AspCoreIdentity.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Services.IService
{
    public interface IRoleService
    {
        Task<IEnumerable<ApplicationRoles>> ListAsync();
        Task<RoleResponse<ApplicationRoles>> GetRoleById(string id);
        Task<RoleResponse<ApplicationRoles>> CreateRoles(ApplicationRoles roles);
        Task<RoleResponse<ApplicationRoles>> UpdateRoles(string id, ApplicationRoles roles);
        Task<RoleResponse<ApplicationRoles>> DeleteRoles(string id);
    }
}
