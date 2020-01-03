using CakeItWebApp.ViewModels;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IHomeService
    {
        Task<CakeIndexViewModel> GetRandomCake();
    }
}
