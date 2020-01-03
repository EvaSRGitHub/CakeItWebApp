using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels;
using CakeWebApp.Services.Common.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class HomeService : IHomeService
    {
        private readonly IRepository<Product> repository;
        private readonly IMapper mapper;

        public HomeService(IRepository<Product> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<CakeIndexViewModel> GetRandomCake()
        {
            CakeIndexViewModel model;

            var max = this.GetCakeProductsCount();

            var rnd = new Random();

            while (true)
            {
                var cakeIdToDisplay = rnd.Next(1, max + 1);

                var product = await this.repository.GetByIdAsync(cakeIdToDisplay);

                if (product == null || (max == 1 && product.IsDeleted == true))
                {
                    return null;
                }

                if (product.IsDeleted == false)
                {
                     model = mapper.Map<Product, CakeIndexViewModel>(product);
                    break;
                }
            }

            return model;
        }

        private int GetCakeProductsCount()
        {
            return this.repository.All().Where(p => p.CategoryId == 1).Count();
        }
    }
}
