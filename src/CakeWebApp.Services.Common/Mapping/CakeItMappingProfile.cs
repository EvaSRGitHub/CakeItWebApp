using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Models.Enums;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Cakes;
using CakeItWebApp.ViewModels.CustomCake;
using CakeItWebApp.ViewModels.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CakeWebApp.Services.Common.Mapping
{
    public class CakeItMappingProfile:Profile
    {
        public CakeItMappingProfile()
        {
            CreateMap<Product, CakeIndexViewModel>().ReverseMap();

            CreateMap<Product, CreateCakeViewModel>().ReverseMap();

            CreateMap<Product, EditAndDeleteViewModel>().ReverseMap();

            CreateMap<Order, OrderViewModel>().ReverseMap();

            CreateMap<OrderDetails, OrderDetailsViewModel>().ReverseMap();

            CreateMap<ShoppingCartItem, OrderedProductsViewModel>().ReverseMap();

            CreateMap<CustomCakeImg, CustomCakeImgViewModel>().ReverseMap();

            CreateMap<CustomCakeOrderViewModel, Ingredients>()
            .ForMember(x => x.Sponge, x => x.MapFrom(y => Enum.Parse(typeof(SpongeType), y.Sponge)))
            .ReverseMap();
        }
    }
}
