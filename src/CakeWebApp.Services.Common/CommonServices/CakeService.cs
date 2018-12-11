﻿using AutoMapper;
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

        public async Task<string> AddCakeToDb(CreateCakeViewModel model)
        {
            var product = this.mapper.Map<CreateCakeViewModel, Product>(model);

            if(this.repository.All().Any(p => p.Name == product.Name))
            {
                return "Cake with such name alreadey exist in the data base."; ;
            }

            this.repository.Add(product);

            try
            {
                await this.repository.SaveChangesAsync();

                return "true";
            }
            catch (Exception e)
            {
                //this.logger.LogError(e.InnerException.ToString());

                return e.InnerException.ToString();
            }
        }

        public bool AddRatingToCake(int cakeId, int rating)
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

        public IEnumerable<CakeIndexViewModel> GetAllCakes()
        {
            return mapper.ProjectTo<CakeIndexViewModel>(this.repository.All()).ToList();
        }

        public async Task<EditAndDeleteViewModel> GetCakeById(int id)
        {
            var product = await this.repository.GetByIdAsync(id);

            var model = this.mapper.Map<Product, EditAndDeleteViewModel>(product);

            return model;
        }

        public async Task<string> UpdateProduct(EditAndDeleteViewModel model)
        {
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
    }
}