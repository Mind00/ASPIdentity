using AspCoreIdentity.Models;
using AspCoreIdentity.Models.incoming;
using AspCoreIdentity.Responses;
using AspCoreIdentity.Services.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspCoreIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IMapper _mapper;

        public ProfileController(IProfileService profileService, IMapper mapper)
        {
            _profileService = profileService;
            _mapper = mapper;
        }

       [HttpPost]
       public async Task<IActionResult> AddFamilyMember([FromBody] FamilyMemberViewModel familyMemgerViewModel)
        {
            if (ModelState.IsValid)
            {
                if(familyMemgerViewModel.PhotoGalleries != null)
                {
                    familyMemgerViewModel.PhotoGallery = new List<PhotoGalleryViewModel>();
                    foreach(var file in familyMemgerViewModel.PhotoGalleries)
                    {
                        var gallery = new PhotoGalleryViewModel()
                        {
                            UrlPath = UploadFiles(file)
                        };
                        familyMemgerViewModel.PhotoGallery.Add(gallery);
                    }
                }
                var dto = _mapper.Map<FamilyMemberViewModel, FamilyMember>(familyMemgerViewModel);
                var result = await _profileService.InsertImage(dto);
                if (result.success)
                {
                    return Ok(result.success);
                }
                return BadRequest(result.Errors);
            }
            else
            {
                return BadRequest("Some Validation error occured.");
            }
        }

        private string UploadFiles(IFormFile file)
        {
            string path = "";
            string fileName = "";
            try
            {
                var extension = "." + file.FileName.Split(".")[file.FileName.Split(".").Length - 1];
                fileName = file.FileName + DateTime.Now.Ticks + extension;
                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\files");
                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }
                path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\files", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyToAsync(stream);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return path ;
        }
    }
}
