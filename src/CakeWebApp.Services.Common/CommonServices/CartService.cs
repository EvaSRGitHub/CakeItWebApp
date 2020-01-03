using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels.Cart;
using CakeWebApp.Services.Common.Cart;
using CakeWebApp.Services.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeWebApp.Services.Common.CommonServices
{
    public class CartService : ICartService
    {
        private readonly IRepository<Product> productsRepo;
        private readonly ICartManager cartManager;
        private IList<Item> cart;


        public CartService(IRepository<Product> productsRepo, ICartManager cartManager)
        {
            this.productsRepo = productsRepo;
            this.cartManager = cartManager;
            this.cart = this.cartManager.GetCartItem();
        }

        public async Task AddToCart(int id)
        {
            var product = await this.productsRepo.GetByIdAsync(id);

            if (product == null)
            {
                throw new NullReferenceException("Product not found");
            }

            if (this.cart == null)
            {
                this.cart = new List<Item>();

                this.cart.Add(new Item { Product = product, Quantity = 1 });
            }
            else
            {
                int index = isExist(product.Id);

                if (index != -1)
                {
                    this.cart[index].Quantity++;
                }
                else
                {
                    this.cart.Add(new Item { Product = product, Quantity = 1 });
                }
            }

            this.cartManager.SetCartItem(this.cart);
        }

        public CartViewModel GetCartItems()
        {
            var model = new CartViewModel();
            model.CartItems = this.cartManager.GetCartItem();

            return model;
        }

        public async Task RemoveFromCart(int id)
        {
            var product = this.productsRepo.All().SingleOrDefault(p => p.Id == id);

            if (product == null)
            {
                throw new NullReferenceException("Product not found");
            }

            if (product.CategoryId == 2)
            {
                this.productsRepo.Delete(product);

                try
                {
                    await this.productsRepo.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw new InvalidOperationException("Sorry, an error occurred while trying to delete custom cake.");
                }
                
            }

            int index = isExist(id);

            if (index >= 0)
            {
                if (this.cart[index].Quantity == 1)
                {
                    this.cart.RemoveAt(index);
                }
                else
                {
                    this.cart[index].Quantity--;
                }
            }

            this.cartManager.SetCartItem(this.cart);
        }

        public void EmptyCart()
        {
            this.ClearCart();
        }

        private void ClearCart()
        {
            this.cart = new List<Item>();

            this.cartManager.SetCartItem(this.cart);
        }

        private int isExist(int id)
        {
            for (int i = 0; i < this.cart.Count; i++)
            {
                if (cart[i].Product.Id.Equals(id))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
