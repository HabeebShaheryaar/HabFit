using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HabFitAPI.Contract;
using HabFitAPI.DTOs;
using HabFitAPI.Entities;
using HabFitAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HabFitAPI.Controllers
{
    [Authorize]
    [Route("api/user/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IUserRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(string userId)
        {
            var photoFromRepo = await _repo.GetPhoto(userId);
            var photo = _mapper.Map<PhotoForReturnDTO>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(string userId, [FromForm]PhotoForCreationDTO photoForCreationDTO)
        {
            if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId);

            var file = photoForCreationDTO.File;
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDTO.Url = uploadResult.Uri.ToString();
            photoForCreationDTO.PublicId = uploadResult.PublicId;
            photoForCreationDTO.UserID = userId;

            var photo = _mapper.Map<Photo>(photoForCreationDTO);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            //userFromRepo.Photos.Add(photo);
            try
            {
                _repo.AddPhoto(photo);
                var photoToReturn = _mapper.Map<PhotoForReturnDTO>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.ID }, photoToReturn);
            }
            catch (Exception)
            {
                return BadRequest("Could not add the photo");
            }
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(string userID, string ID)
        {
            if (userID != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var user = await _repo.GetUser(userID);

            if (!user.Photos.Any(p => p.ID == ID))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(ID);

            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = _repo.GetMainPhotoForUser(userID);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            try
            {
                _repo.SetMainPhoto(photoFromRepo, currentMainPhoto);
                return NoContent();
            }
            catch (Exception)
            {

                return BadRequest("Could not set photo to Main");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(string userID, string ID)
        {
            if (userID != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var user = await _repo.GetUser(userID);

            if (!user.Photos.Any(p => p.ID == ID))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(ID);

            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo");

            try
            {
                if (!string.IsNullOrEmpty(photoFromRepo.PublicID))
                {
                    var deleteParams = new DeletionParams(photoFromRepo.PublicID);

                    var result = _cloudinary.Destroy(deleteParams);

                    if (result.Result == "ok")
                    {
                        _repo.DeletePhoto(photoFromRepo.ID);
                    }
                    else
                    {
                        return BadRequest("Failed to delete the photo in Cloudinary");
                    }
                }
                else
                {
                    _repo.DeletePhoto(photoFromRepo.ID);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to delete the photo");
            }

            return Ok();
        }
    }
}