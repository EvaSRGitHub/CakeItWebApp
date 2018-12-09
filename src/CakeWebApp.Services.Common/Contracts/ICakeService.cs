using CakeItWebApp.ViewModels.Cakes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface ICakeService
    {
        bool AddRatingToJoke(int cakeId, int rating);

        Task<bool> AddCakeToDb(CreateCakeViewModel model);
    }
}
