﻿using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Tutorials;
using CakeWebApp.Services.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class TutorialService : ITutorialService
    {
        private readonly IRepository<Tutorial> repository;
        private readonly IMapper mapper;

        public TutorialService(IRepository<Tutorial> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task AddRatingToCake(int tutorialId, int rating)
        {
            var tutorial = this.repository.All().FirstOrDefault(c => c.Id == tutorialId);

            if (tutorial == null)
            {
                throw new InvalidOperationException("Tutorial not found.");
            }

            if (rating < 1 && rating > 5)
            {
                throw new InvalidOperationException("Invlid rating value.");
            }

            tutorial.Rating = tutorial.Rating + rating ?? rating;

            tutorial.RatingVotes = tutorial.RatingVotes + 1 ?? 1;

            try
            {
                await this.repository.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw new InvalidProgramException(e.Message);
            }
        }

        public async Task AddTutorial(AddTutorialViewModel model)
        {
            if (this.repository.All().Any(t => t.Title == model.Title))
            {
                throw new InvalidOperationException("Tutorial with such title already exists.");
            }

            if (this.repository.All().Any(t => t.Title != model.Title && t.Url == model.Url))
            {
                throw new InvalidOperationException("Tutorial with such url already exists but with other title already exists.");
            }

            var tutorial = this.mapper.Map<AddTutorialViewModel, Tutorial>(model);

            this.repository.Add(tutorial);

            try
            {
                await this.repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new InvalidProgramException(e.Message);
            }
        }

        public async Task<AddTutorialViewModel> GetCakeById(int id)
        {
            var tutorial = await this.repository.GetByIdAsync(id);

            var model = this.mapper.Map<Tutorial, AddTutorialViewModel>(tutorial);

            return model;
        }

        public ICollection<TutorialIndexViewModel> GetTutorials()
        {
            var allTutorials = this.mapper.ProjectTo<TutorialIndexViewModel>(this.repository.All());

            return allTutorials.ToList();
        }

        public async Task UpdateCake(AddTutorialViewModel model)
        {
            if (this.repository.All().Any(t => t.Title == model.Title && t.Id != model.Id))
            {
                throw new InvalidOperationException("Tutorial with such name already exists.");
            }

            if (this.repository.All().Any(p => p.Url == model.Url && p.Id != model.Id))
            {
                throw new InvalidOperationException("Tutorial with such image url already exists.");
            }

            var tutorial = this.mapper.Map<AddTutorialViewModel, Tutorial>(model);

            this.repository.Update(tutorial);

            try
            {
                await this.repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new InvalidProgramException(e.InnerException.ToString());
            }
        }
    }
}
