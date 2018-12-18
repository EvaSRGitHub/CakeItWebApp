using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class CustomCakeService : ICustomCakeService
    {
        private readonly IRepository<Product> repository;
        private readonly IRepository<CustomCakeImg> customCakeImgRepo;
        private readonly IMapper mapper;

        public CustomCakeService(IRepository<Product> repository, IRepository<CustomCakeImg> customCakeImgRepo, IMapper mapper)
        {
            this.repository = repository;
            this.customCakeImgRepo = customCakeImgRepo;
            this.mapper = mapper;
        }

        public CustomCakeOrderViewModel CalculatePrice(CustomCakeOrderViewModel model)
        {
            var side = model.SideDecoration.ToLower();
            var top = model.TopDecoration.ToLower();
            var img = customCakeImgRepo.All().SingleOrDefault(c => c.Side.ToLower() == side && c.Top.ToLower() == top).Img;
            model.Img = img;

            return model;
        }

        //if customer approve the cake CreateProduct - add to Db and make Order

        public async Task AddCustomCakeImg(CustomCakeImgViewModel model)
        {
            var customCakeImg = this.mapper.Map<CustomCakeImgViewModel, CustomCakeImg>(model);

            this.customCakeImgRepo.Add(customCakeImg);

            try
            {
                await this.customCakeImgRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new InvalidProgramException(e.Message);
            }

        }

        public Product CreateCustomProduct(CustomCakeOrderViewModel model)
        {
            var ingredients = this.mapper.Map<CustomCakeOrderViewModel, Ingredients>(model);

            if(ingredients == null)
            {
                throw new InvalidOperationException("Ingredients not set.");
            }

            var product = new Product
            {
                Name = model.TopDecoration + " " + model.SideDecoration,
               CategoryId = 2,
               Image = model.Img,
               Ingredients = ingredients,
               Price = model.Price
            };

            return product;
        }

        public async Task<int?> GetProductId()
        {
            var product = await this.repository.All().LastOrDefaultAsync();

            return product.Id;
        }
    }
}
