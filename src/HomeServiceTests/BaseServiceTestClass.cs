using AutoMapper;
using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.Services.Common.Repository;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Cakes;
using CakeItWebApp.ViewModels.CustomCake;
using CakeItWebApp.ViewModels.Forum;
using CakeItWebApp.ViewModels.Orders;
using CakeItWebApp.ViewModels.Tutorials;
using CakeWebApp.Services.Common.CommonServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CakeItWebApp.Services.Common.Tests
{
    public abstract class BaseServiceTestClass
    {
        protected BaseServiceTestClass()
        {
            this.Mapper = new Mapper(new MapperConfiguration(
                config =>
                {
                    config.CreateMap<Product, CakeIndexViewModel>().ForMember(x => x.Rating, y => y.MapFrom(x => x.Rating ?? 0)).ForMember(x => x.RatingVotes, y => y.MapFrom(x => x.RatingVotes ?? 0)).ReverseMap();
                    config.CreateMap<Product, CreateCakeViewModel>().ReverseMap();
                    config.CreateMap<Product, EditAndDeleteViewModel>().ReverseMap();
                    config.CreateMap<Order, OrderViewModel>().ReverseMap();
                    config.CreateMap<ShoppingCartItem, OrderedProductsViewModel>().ReverseMap();
                    config.CreateMap<OrderDetails, OrderDetailsViewModel>().ReverseMap();
                    config.CreateMap<CustomCakeImgViewModel, CustomCakeImg>().ReverseMap();
                    config.CreateMap<CustomCakeOrderViewModel, Ingredients>().ReverseMap();
                    config.CreateMap<AddTutorialViewModel, Tutorial>().ReverseMap();
                    config.CreateMap<TutorialIndexViewModel, Tutorial>().ReverseMap();
                    config.CreateMap<Comment, CommentInputViewModel>().ReverseMap();
                    config.CreateMap<Comment, PostIndexViewModel>().ReverseMap();
                }));
        }

        protected IMapper Mapper { get; private set; }

    }
}
