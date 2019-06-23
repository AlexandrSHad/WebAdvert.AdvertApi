using AutoMapper;
using WebAdvert.AdvertApi.Models;

namespace WebAdvert.AdvertApi.Services
{
    public class AdvertMappings : Profile
    {
        public AdvertMappings()
        {
            CreateMap<AdvertModel, AdvertDbModel>();
        }
    }
}
