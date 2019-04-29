using AutoMapper;
using HabFit.Common.Helpers;
using HabFitAPI.DTOs;
using HabFitAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HabFitAPI.Helpers
{
    public class AutoMapperProfiles : Profile 
    {
        public AutoMapperProfiles()
        {
            CreateMap<Users, UserForListDTO>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).URL);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.MapFrom((d, dest) => d.DateOfBirth.CalculateAge());
                });

            CreateMap<Users, UserForDetailedDTO>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).URL);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.MapFrom((d, dest) => d.DateOfBirth.CalculateAge());
                });
            CreateMap<UserForUpdateDTO, Users>();
            CreateMap<Photo, PhotoForReturnDTO>();
            CreateMap<PhotoForCreationDTO, Photo>();
        }
    }
}
