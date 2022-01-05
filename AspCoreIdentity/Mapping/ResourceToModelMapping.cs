using AspCoreIdentity.Models;
using AspCoreIdentity.Models.incoming;
using AspCoreIdentity.Responses;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Mapping
{
    public class ResourceToModelMapping : Profile
    {
        public ResourceToModelMapping()
        {
            CreateMap<registerViewModel, ApplicationUser>();
            CreateMap<UserLoginRequest, ApplicationUser>().ForMember(x => x.PasswordHash, y => y.MapFrom(z => z.Password));
            CreateMap<PhotoGalleryViewModel, PhotoGallery>();
        }
    }
}
