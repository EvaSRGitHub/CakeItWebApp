using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Cakes;
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
        }
    }
}
