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
using System.Text;
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

            if (this.repository.All().Any(p => p.Name == product.Name && product.IsDeleted == true))
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
                throw new InvalidOperationException( e.InnerException.Message);
            }
        }

        public async Task AddRatingToCake(int cakeId, int rating)
        {
            var cake = this.repository.All().FirstOrDefault(c => c.Id == cakeId);

            if (cake == null)
            {
                throw new InvalidOperationException("Product not found.");
            }

            if (rating < 1 && rating > 5)
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

                throw new InvalidProgramException(e.Message);
            }
        }

        public async Task DeleteCake(int id)
        {
            if (!this.repository.All().Any(p => p.Id == id))
            {
                return;
            }

            var product = await this.repository.GetByIdAsync(id);

            this.repository.Delete(product);

            await this.repository.SaveChangesAsync();
        }

        public IEnumerable<CakeIndexViewModel> GetAllCakes() 
        {
            return mapper.ProjectTo<CakeIndexViewModel>(this.repository.All().Where(c => c.IsDeleted == false)) .ToList();
        }

        public async Task<EditAndDeleteViewModel> GetCakeById(int id)
        {
            var product = await this.repository.GetByIdAsync(id);

            var model = this.mapper.Map<Product, EditAndDeleteViewModel>(product);

            return model;
        }

        public async Task<CakeIndexViewModel> ShowCakeDetails(int id)
        {
            var cake = await this.repository.GetByIdAsync(id);

            var cakeDetails = this.mapper.Map<Product, CakeIndexViewModel>(cake);

            return cakeDetails;
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
                return e.InnerException.ToString();
            }
            return "true";
        }

        public async Task SoftDelete(int id)
        {
            if (!this.repository.All().Any(p => p.Id == id))
            {
                return;
            }

            var product = await this.repository.GetByIdAsync(id);

            product.IsDeleted = true;

            await this.repository.SaveChangesAsync();
        }
    }
}
