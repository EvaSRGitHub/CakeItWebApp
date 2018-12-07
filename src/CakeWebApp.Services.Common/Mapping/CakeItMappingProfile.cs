using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Text;

namespace CakeWebApp.Services.Common.Mapping
{
    public class CakeItMappingProfile:Profile
    {
        public CakeItMappingProfile()
        {
            CreateMap<Product, HomeIndexViewModel>().ReverseMap();
        }
    }
}
