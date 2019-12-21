using CakeItWebApp.ViewModels;
using CakeWebApp.Services.Common.Contracts;
using System.Collections.Generic;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class ErrorService : IErrorService
    {
        public object ErrorParm { get; private set; }

        public void PassErrorParam(object param)
        {
            this.ErrorParm = param;
        }

        public ErrorViewModel GetErrorModel(ICollection<string> errors)
        {
            return new ErrorViewModel { Errors = errors };
        }
    }
}
