using CakeItWebApp.Models;
using System.Collections.Generic;

namespace CakeWebApp.Services.Common.Cart
{
    public interface ICartManager
    {
        IList<Item> GetCartItem();
        void SetCartItem(object value);
    }
}
