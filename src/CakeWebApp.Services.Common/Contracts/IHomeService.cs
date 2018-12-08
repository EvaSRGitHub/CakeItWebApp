using CakeItWebApp.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IHomeService
    {
        Task<HomeIndexViewModel> GetRandomCake();

        int GetCakeProductsCount();
    }
}
