using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Cakes;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class CakeService:ICakeService
    {
        private readonly ILogger<CakeService> logger;
        private readonly IRepository<Product> repository;
        private readonly IMapper mapper;

        public CakeService(ILogger<CakeService> logger, IRepository<Product> repository, IMapper mapper)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<bool> AddCakeToDb(CreateCakeViewModel model)
        {
            var product = this.mapper.Map<CreateCakeViewModel, Product>(model);

            this.repository.Add(product);

            try
            {
                await this.repository.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.InnerException.ToString());
                return false;
            }
        }
        public bool AddRatingToJoke(int cakeId, int rating)
        {
            var cake = this.repository.All().FirstOrDefault(j => j.Id == cakeId);
            if (cake != null)
            {
                cake.Rating += rating;
                this.repository.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
