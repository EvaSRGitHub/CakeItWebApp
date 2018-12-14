using CakeItWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IErrorService
    {
        object ErrorParm { get; }
        
        //work for pages
        void PassErrorParam(object param);

        //work for mvc
        ErrorViewModel GetErrorModel(List<string> errors);
    }
}
