﻿using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class CustomCakeService : ICustomCakeService
    {
        private readonly IRepository<Product> repository;
        private readonly IRepository<CustomCakeImg> customCakeImgRepo;
        private readonly IMapper mapper;
        private readonly ILogger<CustomCakeService> logger;

        public CustomCakeService(IRepository<Product> repository, IRepository<CustomCakeImg> customCakeImgRepo, IMapper mapper, ILogger<CustomCakeService> logger)
        {
            this.repository = repository;
            this.customCakeImgRepo = customCakeImgRepo;
            this.mapper = mapper;
            this.logger = logger;
        }

        public CustomCakeOrderViewModel AssignImgAndPrice(CustomCakeOrderViewModel model)
        {
            var side = model.SideDecoration.ToLower();

            var top = model.TopDecoration.ToLower();

            var img = customCakeImgRepo.All().SingleOrDefault(c => c.Side.ToLower() == side && c.Top.ToLower() == top).Img;

            model.Img = img;

            if(!Uri.TryCreate(model.Img, UriKind.Absolute, out Uri result))
            {
                throw new InvalidOperationException("Sorry. Error occurred while processing your order. Please, contact us.");
            }
           
            return model;
        }

        public async Task AddCustomCakeImg(CustomCakeImgViewModel model)
        {
            if(this.customCakeImgRepo.All().Any(c => c.Name == model.Name))
            {
                throw new InvalidOperationException("Cake with such name already exists.");
            }

            var customCakeImg = this.mapper.Map<CustomCakeImgViewModel, CustomCakeImg>(model);

            this.customCakeImgRepo.Add(customCakeImg);

            try
            {
                await this.customCakeImgRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                throw new InvalidOperationException("Sorry, an error occurred and your request couldn't be processed.");
            }

        }

        public Product CreateCustomProduct(CustomCakeOrderViewModel model)
        {
            var ingredients = this.mapper.Map<CustomCakeOrderViewModel, Ingredients>(model);

            if(ingredients == null)
            {
                throw new AutoMapperMappingException("Ingredients not set.");
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

            int? id = product.Id;

            if (id == null)
            {
                throw new NullReferenceException("Product not Found");
            }

            return id;
        }

        public IEnumerable<CustomCakeImgViewModel> GetAllCustomCakesImg()
        {
            return this.mapper.ProjectTo<CustomCakeImgViewModel>(this.customCakeImgRepo.All());
        }

        public async Task<CustomCakeImgViewModel> GetCustomCakeImgById(int id)
        {
            var customCakeImg = await this.customCakeImgRepo.GetByIdAsync(id);

            if (customCakeImg == null)
            {
                throw new InvalidOperationException("Custom cake image not found.");
            }

            var model = this.mapper.Map<CustomCakeImg, CustomCakeImgViewModel>(customCakeImg);

            return model;
        }

        public async Task UpdateCustomCakeImg(CustomCakeImgViewModel model)
        {
            if (this.customCakeImgRepo.All().Any(t => t.Name == model.Name && t.Id != model.Id))
            {
                throw new InvalidOperationException("Custom Cake with such name already exists.");
            }

            if (this.customCakeImgRepo.All().Any(p => p.Img == model.Img && p.Id != model.Id))
            {
                throw new InvalidOperationException("Custom Cake with such image url already exists.");
            }

            var customCakeImg = this.mapper.Map<CustomCakeImgViewModel, CustomCakeImg>(model);

            this.customCakeImgRepo.Update(customCakeImg);

            try
            {
                await this.customCakeImgRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                throw new InvalidOperationException("Sorry, an error occurred and your request couldn't be processed.");
            }
        }

        public async Task DeleteCustomCakeImg(int id)
        {
            var customCakeImg = await this.customCakeImgRepo.GetByIdAsync(id);

            if (customCakeImg == null)
            {
                throw new InvalidOperationException("Tutorial not found.");
            }

            this.customCakeImgRepo.Delete(customCakeImg);

            try
            {
                await this.customCakeImgRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred and your request couldn't be processed.");
            }
        }
    }
}