using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Tutorials;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class TutorialService : ITutorialService
    {
        private readonly IRepository<Tutorial> repository;
        private readonly IMapper mapper;
        private readonly ILogger<TutorialService> logger;

        public TutorialService(IRepository<Tutorial> repository, IMapper mapper, ILogger<TutorialService> logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task AddRatingToTutorial(int tutorialId, int rating)
        {
            var tutorial = this.repository.All().FirstOrDefault(c => c.Id == tutorialId);

            if (tutorial == null)
            {
                throw new NullReferenceException("Tutorial not found.");
            }

            if (rating < 1 || rating > 5)
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
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred and your request couldn't be processed.");
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
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred and your request couldn't be processed.");
            }
        }

        public async Task DeleteTutorial(int id)
        {
            var tutorial = await this.repository.GetByIdAsync(id);

            if(tutorial == null)
            {
                throw new NullReferenceException("Tutorial not found.");
            }

            this.repository.Delete(tutorial);

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

        public async Task<AddTutorialViewModel> GetTutorialById(int id)
        {
            var tutorial = await this.repository.GetByIdAsync(id);

            if (tutorial == null)
            {
                throw new NullReferenceException("Tutorial not found.");
            }

            var model = this.mapper.Map<Tutorial, AddTutorialViewModel>(tutorial);

            return model;
        }

        public ICollection<TutorialIndexViewModel> GetTutorials()
        {
            var allTutorials = this.mapper.ProjectTo<TutorialIndexViewModel>(this.repository.All());

            return allTutorials.ToList();
        }

        public async Task UpdateTutorial(AddTutorialViewModel model)
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
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry, an error occurred and your request couldn't be processed.");
            }
        }
    }
}
