using CakeItWebApp.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CakeWebApp.Services.Common.Cart
{
    public class CartSessionWrapper : ICartSessionWrapper
    {
        private const string Key = "cart";
        private IHttpContextAccessor accessor;
        private ISession session;

        public CartSessionWrapper(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
            this.session = this.accessor.HttpContext.Session;
        }

        public IList<Item> GetJson()
        {
            return SessionHelper.GetObjectFromJson<IList<Item>>(this.session, Key) ?? new List<Item>();
        }

        public void SetJson(object value)
        {
            SessionHelper.SetObjectAsJson(this.session, Key, value);
        }
    }
}
