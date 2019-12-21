using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Tags;
using CakeWebApp.Services.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class TagService : ITagService
    {
        private readonly ILogger<TagService> logger;
        private readonly IRepository<Tag> tagRepo;
        private readonly IMapper mapper;

        public TagService(ILogger<TagService> logger, IRepository<Tag> tagRepo, IMapper mapper)
        {
            this.logger = logger;
            this.tagRepo = tagRepo;
            this.mapper = mapper;
        }

        public async Task CreateTag(TagInputViewModel model)
        {
            var tag = this.mapper.Map<TagInputViewModel, Tag>(model);

            if (this.tagRepo.All().Any(t => t.Name == tag.Name && tag.IsDeleted == false))
            {
                throw new InvalidOperationException("Tag with such name alreadey exist in the data base.");
            }

            tagRepo.Add(tag);

            try
            {
                await tagRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);

                throw new InvalidOperationException("Sorry an error accored while creating a tag.");
            }
        }

        public IQueryable<TagInputViewModel> GetAllTags()
        {
            var tags = this.mapper.ProjectTo<TagInputViewModel>(this.tagRepo.All());

            return tags;
        }

        public async Task<TagInputViewModel> GetTagById(int id)
        {
            var tagModel = await this.tagRepo.GetByIdAsync(id);

            if (tagModel == null)
            {
                throw new NullReferenceException("Tag not found");
            }

            return this.mapper.Map<Tag, TagInputViewModel>(tagModel);
        }

        public async Task UpdateTag(TagInputViewModel model)
        {
            if (this.tagRepo.All().Any(t => t.Name == model.Name && t.Id != model.Id))
            {
                throw new InvalidOperationException("Tag with the same name already exists.");
            }

            var tag = this.mapper.Map<TagInputViewModel, Tag>(model);

            this.tagRepo.Update(tag);

            try
            {
                await this.tagRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.Message);
                throw new InvalidOperationException("Sorry, can't update tag.");
            }
        }
    }
}
