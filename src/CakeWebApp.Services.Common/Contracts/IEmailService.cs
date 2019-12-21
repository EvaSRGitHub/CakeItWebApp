using CakeItWebApp.ViewModels.Orders;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IEmailService
    {
        Task SendOrderMail(OrderDetailsViewModel model);
    }
}
