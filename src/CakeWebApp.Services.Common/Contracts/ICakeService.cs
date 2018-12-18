using CakeItWebApp.Models;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Cakes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface ICakeService
    {
        Task AddRatingToCake(int cakeId, int rating);

        Task AddCakeToDb<T>(T model);

        IEnumerable<CakeIndexViewModel> GetAllCakes();

        Task<EditAndDeleteViewModel> GetCakeById(int id);

        Task<string> UpdateCake(EditAndDeleteViewModel model);

        Task DeleteCake(int id);

        Task<CakeIndexViewModel> ShowCakeDetails(int id);

        Task SoftDelete(int id);
    }
}
