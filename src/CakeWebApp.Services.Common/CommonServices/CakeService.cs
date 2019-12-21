using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Cakes;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class CakeService : ICakeService
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

        public async Task AddCakeToDb<T>(T model)
        {
            var product = this.mapper.Map<T, Product>(model);

            if (this.repository.All().Any(p => p.Name == product.Name && product.IsDeleted == false))
            {
                throw new InvalidOperationException("Cake with such name alreadey exist in the data base."); 
            }

            this.repository.Add(product);

            try
            {
                await this.repository.SaveChangesAsync();

            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred and your request couldn't be processed.");
            }
        }

        public async Task AddRatingToCake(int cakeId, int rating)
        {
            var cake = this.repository.All().FirstOrDefault(c => c.Id == cakeId);

            if (cake == null)
            {
                throw new NullReferenceException("Product not found.");
            }

            if (rating < 1 || rating > 5)
            {
                throw new InvalidOperationException("Invlid rating value.");
            }

            cake.Rating = cake.Rating + rating ?? rating;

            cake.RatingVotes = cake.RatingVotes + 1 ?? 1;

            try
            {
                await this.repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred and your request couldn't be processed.");
            }
        }

        public async Task DeleteCake(EditAndDeleteViewModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Cake not found.");
            }

            var cake = await this.repository.GetByIdAsync(model.Id);
            cake.IsDeleted = true;

            this.repository.Update(cake);

            try
            {
                await this.repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred while trying to delete cake.");
            }
        }

        public IEnumerable<CakeIndexViewModel> GetAllActiveCakes() 
        {

            return mapper.ProjectTo<CakeIndexViewModel>(this.repository.All().Where(c => c.IsDeleted == false)) .ToList();
        }

        public async Task<EditAndDeleteViewModel> GetCakeById(int id)
        {
            var product = await this.repository.GetByIdAsync(id);

            if(product == null || product.IsDeleted == true)
            {
                throw new NullReferenceException("Cake not found.");
            }

            var model = this.mapper.Map<Product, EditAndDeleteViewModel>(product);

            return model;
        }

        public async Task<CakeIndexViewModel> ShowCakeDetails(int id)
        {
            var cake = await this.repository.GetByIdAsync(id);

            var cakeDetails = this.mapper.Map<Product, CakeIndexViewModel>(cake);

            return cakeDetails;
        }

        public async Task<EditAndDeleteViewModel> GetCakeToEdit(int id)
        {
            var product = await this.repository.GetByIdAsync(id);

            if (product == null)
            {
                throw new NullReferenceException("Cake not found.");
            }

            var model = this.mapper.Map<Product, EditAndDeleteViewModel>(product);

            return model;
        }

        public async Task<string> UpdateCake(EditAndDeleteViewModel model)
        {
            if (this.repository.All().Any(p => p.Name == model.Name && p.Id != model.Id))
            {
                return "Product with such name already exists.";
            }

            if (this.repository.All().Any(p => p.Image == model.Image && p.Id != model.Id))
            {
                return "Product with such image url already exists.";
            }

            var product = this.mapper.Map<EditAndDeleteViewModel, Product>(model);

            this.repository.Update(product);

            try
            {
                await this.repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                return "Sorry, an error occurred and your request couldn't be processed.";
            }

            return "true";
        }

        public async Task SoftDelete(int id)
        {
            if (!this.repository.All().Any(p => p.Id == id))
            {
                throw new InvalidOperationException("Cake not found.");
            }

            var product = await this.repository.GetByIdAsync(id);

            product.IsDeleted = true;

            await this.repository.SaveChangesAsync();
        }

        public IEnumerable<CakeIndexViewModel> GetAllCakes()
        {
            var allCakes = mapper.ProjectTo<CakeIndexViewModel>(this.repository.All()).ToList();

            return allCakes;
        }
    }
}
