using CakeItWebApp.ViewModels.Tutorials;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface ITutorialService
    {
        Task AddTutorial(AddTutorialViewModel model);

        ICollection<TutorialIndexViewModel> GetTutorials();

        Task AddRatingToTutorial(int tutorialId, int rating);

       Task<AddTutorialViewModel> GetTutorialById(int id);

        Task UpdateTutorial(AddTutorialViewModel model);

        Task DeleteTutorial(int id);
    }
}
