using CakeItWebApp.Models;
using System.Collections.Generic;

namespace CakeWebApp.Services.Common.Cart
{
    public interface ICartSessionWrapper
    {
        IList<Item> GetJson();

        void SetJson(object value);
    }
}
