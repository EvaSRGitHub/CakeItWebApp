using CakeItWebApp.ViewModels.Tags;
using System.Linq;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface ITagService
    {
        Task CreateTag(TagInputViewModel model);

        IQueryable<TagInputViewModel> GetAllTags();

        Task<TagInputViewModel> GetTagById(int id);

        Task UpdateTag(TagInputViewModel model);
    }
}
