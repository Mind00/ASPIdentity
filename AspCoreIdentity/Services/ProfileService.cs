using AspCoreIdentity.Context;
using AspCoreIdentity.Models;
using AspCoreIdentity.Responses;
using AspCoreIdentity.Services.IService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<GenericResponse> InsertImage(FamilyMember familyMember)
        {
            try
            {
                var newFamily = new FamilyMember()
                {
                    FName = familyMember.FName,
                    Relation = familyMember.Relation,
                    AddedBy = "1",
                    AddedTo = DateTime.UtcNow
                };

                familyMember.PhotoGallery = new List<PhotoGallery>();
                foreach (var file in familyMember.PhotoGallery)
                {
                    newFamily.PhotoGallery.Add(new PhotoGallery()
                    {
                        Name = file.Name,
                        UrlPath = file.UrlPath
                    });
                }
                await _context.FamilyMembers.AddAsync(newFamily);
                await _context.SaveChangesAsync();
                return new GenericResponse
                {
                    success = true,
                    Message = "Record Added Successfully."
                };
            } catch(Exception ex)
            {
                return new GenericResponse
                {
                    Errors = new List<string>()
                    {
                        ex.Message
                    }
                };
            }
            
            
        }

        
    }
}
