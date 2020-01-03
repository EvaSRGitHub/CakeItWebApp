using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.CustomCake;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if(model.Sponge.Contains("Choose") || model.FirstLayerCream.Contains("Choose") || model.SecondLayerCream.Contains("Choose") || model.Filling.Contains("Choose") || model.SideDecoration.Contains("Choose") || model.TopDecoration.Contains("Choose"))
            {
                throw new InvalidOperationException("Please fill the form correctlly.");
            }

            var side = model.SideDecoration.ToLower();

            var top = model.TopDecoration.ToLower();

            var customCakeImg = customCakeImgRepo.All().SingleOrDefault(c => c.Side.ToLower() == side && c.Top.ToLower() == top);

            if(customCakeImg == null)
            {
                throw new NullReferenceException("Sorry, we are out of this product. Please choose other Side - Top combination.");
            }

            model.Img = customCakeImg.Img;

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
                this.logger.LogDebug(e.Message);

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
                Name = model.TopDecoration.Replace('_', ' ') + " with " + model.SideDecoration.Replace('_', ' '),
               CategoryId = 2,
               Image = model.Img,
               Ingredients = ingredients,
               Price = model.Price
            };

            return product;
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
                throw new NullReferenceException("Custom cake image not found.");
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
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred and your request couldn't be processed.");
            }
        }

        public async Task DeleteCustomCakeImg(CustomCakeImgViewModel model)
        {
            var customCakeImg = await this.customCakeImgRepo.GetByIdAsync(model.Id);

            this.customCakeImgRepo.Delete(customCakeImg);

            try
            {
                await this.customCakeImgRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred while trying to delete custom cake.");
            }
        }
    }
}
