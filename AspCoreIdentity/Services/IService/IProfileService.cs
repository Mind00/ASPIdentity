using AspCoreIdentity.Models;
using AspCoreIdentity.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Services.IService
{
    public interface IProfileService
    {
        Task<GenericResponse> InsertImage( FamilyMember familyMember);
    }
}
