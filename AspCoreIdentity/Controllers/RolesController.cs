using AspCoreIdentity.Models;
using AspCoreIdentity.Models.incoming;
using AspCoreIdentity.Models.outgoing;
using AspCoreIdentity.Services.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RolesController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<RoleResource>> RolesList()
        {
            var list = await _roleService.ListAsync();
            var dto = _mapper.Map<IEnumerable<ApplicationRoles>, IEnumerable<RoleResource>>(list);
            return dto;
        }

        [HttpGet("{id}")]
        public async Task<RoleResource> GetRoleById(string id)
        {

            var list = await _roleService.GetRoleById(id);
            var dto = _mapper.Map<ApplicationRoles, RoleResource>(list.Data);
            return dto;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoles([FromBody] RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var roleDto = _mapper.Map<RoleViewModel, ApplicationRoles>(roleViewModel);
                var newRole = await _roleService.CreateRoles(roleDto);
                if (newRole.isSuccess)
                {
                    return Ok(newRole.Message);
                }
                return BadRequest(newRole.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateRole([FromBody] RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var roleDto = _mapper.Map<RoleViewModel, ApplicationRoles>(roleViewModel);
                var updatedRole = await _roleService.UpdateRoles(roleViewModel.Id, roleDto);
                if (updatedRole.isSuccess)
                {
                    return Ok(updatedRole.Message);
                }
                return BadRequest(updatedRole.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var deletedRole = await _roleService.DeleteRoles(id);
            if (deletedRole.isSuccess)
            {
                return Ok(deletedRole.Message);
            }
            return BadRequest(deletedRole.Errors);
        }
    }
}
