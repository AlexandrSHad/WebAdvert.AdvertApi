using AutoMapper;
using WebAdvert.AdvertApi.Dto;

namespace WebAdvert.AdvertApi.Services
{
    public class AdvertMappings : Profile
    {
        public AdvertMappings()
        {
            CreateMap<AdvertDto, AdvertDbModel>();
            CreateMap<AdvertDbModel, AdvertDto>();
        }
    }
}
