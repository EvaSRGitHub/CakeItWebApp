using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.Models.Enums;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Books;
using CakeItWebApp.ViewModels.Cakes;
using CakeItWebApp.ViewModels.CustomCake;
using CakeItWebApp.ViewModels.Forum;
using CakeItWebApp.ViewModels.Orders;
using CakeItWebApp.ViewModels.Tags;
using CakeItWebApp.ViewModels.Tutorials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CakeWebApp.Services.Common.Mapping
{
    public class CakeItMappingProfile:Profile
    {
        public CakeItMappingProfile()
        {
            CreateMap<Product, CakeIndexViewModel>().ForMember(x => x.Rating, y => y.MapFrom(x => x.Rating ?? 0)).ForMember(x => x.RatingVotes, y => y.MapFrom(x => x.RatingVotes ?? 0)).ReverseMap();

            CreateMap<Product, CreateCakeViewModel>().ReverseMap();

            CreateMap<Product, EditAndDeleteViewModel>().ReverseMap();

            CreateMap<Order, OrderViewModel>().ReverseMap();

            CreateMap<OrderDetails, OrderDetailsViewModel>().ReverseMap();

            CreateMap<ShoppingCartItem, OrderedProductsViewModel>().ReverseMap();

            CreateMap<CustomCakeImg, CustomCakeImgViewModel>().ReverseMap();

            CreateMap<CustomCakeOrderViewModel, Ingredients>().ReverseMap();

            CreateMap<Tutorial, AddTutorialViewModel>().ReverseMap();

            CreateMap<Tutorial, TutorialIndexViewModel>().ReverseMap();

            CreateMap<Tag, TagInputViewModel>().ReverseMap();

            CreateMap<Comment, CommentInputViewModel>().ReverseMap();

            CreateMap<Book, CreateBookViewModel>().ReverseMap();

            CreateMap<Book, BookIndexViewModel>().ReverseMap();


        }
    }
}
