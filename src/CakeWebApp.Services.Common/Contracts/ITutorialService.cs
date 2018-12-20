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

        Task AddRatingToCake(int tutorialId, int rating);

       Task<AddTutorialViewModel> GetCakeById(int id);

        Task UpdateCake(AddTutorialViewModel model);

    }
}
