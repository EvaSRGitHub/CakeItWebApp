using System;
using System.Collections.Generic;
using System.Text;

namespace CakeWebApp.Services.Common.Contracts
{
    public interface IErrorService
    {
        void PassErrorParam(object param);

        object ErrorParm { get; }
    }
}
