using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAdvert.AdvertApi.Dto;

namespace WebAdvert.AdvertApi.Services
{
    // TODO: inject DynamoDBContext or it wrapper as a constructor
    public class DynamoDbAdvertStorage : IAdvertStorageService
    {
        private readonly IMapper _mapper;

        public DynamoDbAdvertStorage(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<string> AddAsync(AdvertDto model)
        {
            var dbModel = _mapper.Map<AdvertDbModel>(model);

            dbModel.Id = Guid.NewGuid().ToString();
            dbModel.CreationDateTime = DateTime.UtcNow;
            dbModel.Status = AdvertStatus.Pending;

            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync(dbModel);
                }
            }

            return dbModel.Id;
        }

        public async Task ConfirmAsync(ConfirmAdvertDto model)
        {
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    var advert = await context.LoadAsync<AdvertDbModel>(model.Id);

                    if (advert == null)
                    {
                        throw new KeyNotFoundException($"Record with Id: {model.Id} was not found.");
                    }

                    if (model.Status == AdvertStatus.Active)
                    {
                        advert.Status = AdvertStatus.Active;
                        advert.FilePath = model.FilePath;
                        await context.SaveAsync(advert);
                    }
                    else
                    {
                        await context.DeleteAsync(advert);
                    }
                }
            }
        }

        public async Task<AdvertDto> GetByIdAsync(string id)
        {
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    var advert = await context.LoadAsync<AdvertDbModel>(id);
                    return _mapper.Map<AdvertDto>(advert);
                }
            }
        }

        public async Task<bool> CheckHealthAsync()
        {
            using (var client = new AmazonDynamoDBClient())
            {
                var tableInfo = await client.DescribeTableAsync("Adverts");
                return tableInfo.Table.TableStatus == TableStatus.ACTIVE;
            }
        }
    }
}
