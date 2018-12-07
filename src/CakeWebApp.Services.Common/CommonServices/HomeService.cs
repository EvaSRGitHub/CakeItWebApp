using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Home;
using CakeWebApp.Services.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<HomeIndexViewModel> GetCakeById(int id)
        {
            var product = await this.repository.GetByIdAsync(id);

            var model = mapper.Map<Product, HomeIndexViewModel>(product);

            return model;
        }

        public int GetCakeProductsCount()
        {
            return this.repository.All().Where(p => p.Category.Type.ToString() == "Cake").Count();
        }
    }
}
