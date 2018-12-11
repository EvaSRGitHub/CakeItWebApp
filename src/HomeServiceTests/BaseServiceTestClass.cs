﻿using AutoMapper;
using CakeItWebApp.Data;
using CakeItWebApp.Models;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Cakes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CakeItWebApp.Services.Common.Tests
{
    public abstract class BaseServiceTestClass
    {
        protected BaseServiceTestClass()
        {
            this.Mapper = new Mapper(new MapperConfiguration(
                config => {
                    config.CreateMap<Product, CakeIndexViewModel>().ReverseMap();
                    config.CreateMap<Product, CreateCakeViewModel>().ReverseMap();
                    config.CreateMap<Product, EditAndDeleteViewModel>().ReverseMap();
                }));

            this.Options = new DbContextOptionsBuilder<CakeItDbContext>().UseInMemoryDatabase(databaseName: "CakeItInMemory").Options;

            this.Db = new CakeItDbContext(this.Options);
        }

        protected IMapper Mapper { get; private set; }

        protected DbContextOptions<CakeItDbContext> Options { get; private set; }

        public CakeItDbContext Db { get; protected set; }
    }
}