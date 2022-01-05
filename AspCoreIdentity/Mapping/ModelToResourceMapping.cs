using AspCoreIdentity.Models;
using AspCoreIdentity.Models.incoming;
using AspCoreIdentity.Models.outgoing;
using AspCoreIdentity.Responses;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Mapping
{
    public class ModelToResourceMapping : Profile
    {
        public ModelToResourceMapping()
        {

            CreateMap<ApplicationUser, UserDto>();
            CreateMap<ApplicationRoles, RoleResource>();
        }
    }
}
