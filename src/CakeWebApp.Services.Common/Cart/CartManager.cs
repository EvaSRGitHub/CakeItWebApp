using CakeItWebApp.Models;
using System.Collections.Generic;

namespace CakeWebApp.Services.Common.Cart
{
    public class CartManager : ICartManager
    {
        private ICartSessionWrapper wrapper;
        public CartManager(ICartSessionWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        public IList<Item> GetCartItem()
        {
            return this.wrapper.GetJson();
        }

        public void SetCartItem(object value)
        {
            this.wrapper.SetJson(value);
        }
    }
}
