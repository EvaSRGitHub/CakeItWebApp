using CakeItWebApp.Models;
using CakeItWebApp.ViewModels.CustomCake;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface ICustomCakeService
    {
        CustomCakeOrderViewModel AssignImgAndPrice(CustomCakeOrderViewModel model);

        Task AddCustomCakeImg(CustomCakeImgViewModel model);

        Product CreateCustomProduct(CustomCakeOrderViewModel model);

        IEnumerable<CustomCakeImgViewModel> GetAllCustomCakesImg();

        Task<CustomCakeImgViewModel> GetCustomCakeImgById(int id);

        Task UpdateCustomCakeImg(CustomCakeImgViewModel model);

        Task DeleteCustomCakeImg(CustomCakeImgViewModel model);
    }
}
