using CakeItWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IHomeService
    {
        Task<CakeIndexViewModel> GetRandomCake();

        int GetCakeProductsCount();
    }
}
