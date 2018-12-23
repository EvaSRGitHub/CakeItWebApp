using CakeItWebApp.Models;
using CakeItWebApp.ViewModels.CustomCake;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface ICustomCakeService
    {
        CustomCakeOrderViewModel AssignImgAndPrice(CustomCakeOrderViewModel model);

        Task AddCustomCakeImg(CustomCakeImgViewModel model);

        Product CreateCustomProduct(CustomCakeOrderViewModel model);

        Task<int?> GetProductId();

        IEnumerable<CustomCakeImgViewModel> GetAllCustomCakesImg();

        Task<CustomCakeImgViewModel> GetCustomCakeImgById(int id);

        Task UpdateCustomCakeImg(CustomCakeImgViewModel model);

        Task DeleteCustomCakeImg(int id);
    }
}
