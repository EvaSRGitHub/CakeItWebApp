using CakeItWebApp.ViewModels;
using System.Collections.Generic;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IErrorService
    {
        object ErrorParm { get; }
        
        //work for pages
        void PassErrorParam(object param);

        //work for mvc
        ErrorViewModel GetErrorModel(ICollection<string> errors);
    }
}
