using System.Threading.Tasks;
using WebAdvert.AdvertApi.Dto;

namespace WebAdvert.AdvertApi.Services
{
    public interface IAdvertStorageService
    {
        Task<string> AddAsync(AdvertDto model);
        Task ConfirmAsync(ConfirmAdvertDto model);
        Task<AdvertDto> GetByIdAsync(string id);
        Task<bool> CheckHealthAsync();
    }
}
