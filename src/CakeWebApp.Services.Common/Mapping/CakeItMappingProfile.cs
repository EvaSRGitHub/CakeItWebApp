using AutoMapper;
using CakeItWebApp.Models;
using CakeItWebApp.ViewModels;
using CakeItWebApp.ViewModels.Books;
using CakeItWebApp.ViewModels.Cakes;
using CakeItWebApp.ViewModels.CustomCake;
using CakeItWebApp.ViewModels.Forum;
using CakeItWebApp.ViewModels.Orders;
using CakeItWebApp.ViewModels.Tags;
using CakeItWebApp.ViewModels.Tutorials;
using System.Linq;

namespace CakeWebApp.Services.Common.Mapping
{
    public class CakeItMappingProfile : Profile
    {
        public CakeItMappingProfile()
        {
            CreateMap<Product, CakeIndexViewModel>().ForMember(x => x.Rating, y => y.MapFrom(x => x.Rating ?? 0)).ForMember(x => x.RatingVotes, y => y.MapFrom(x => x.RatingVotes ?? 0)).ReverseMap();

            CreateMap<Product, CreateCakeViewModel>().ReverseMap();

            CreateMap<Product, EditAndDeleteViewModel>().ReverseMap();

            CreateMap<Order, OrderViewModel>().ReverseMap();

            CreateMap<OrderDetails, OrderDetailsViewModel>().ReverseMap();

            CreateMap<CustomCakeImg, CustomCakeImgViewModel>().ReverseMap();

            CreateMap<CustomCakeOrderViewModel, Ingredients>().ReverseMap();

            CreateMap<Tutorial, AddTutorialViewModel>().ReverseMap();

            CreateMap<Tutorial, TutorialIndexViewModel>().ReverseMap();

            CreateMap<Tag, TagInputViewModel>().ReverseMap();

            CreateMap<Comment, CommentInputViewModel>().ReverseMap();

            CreateMap<Comment, EditCommentViewModel>()
                .ForMember(x => x.PostAuthorName, y => y.MapFrom(x => x.Post.Author.UserName))
                .ForMember(x => x.PostId, y => y.MapFrom(x => x.PostId))
                .ForMember(x => x.FullContent, y => y.MapFrom(x => x.Post.FullContent))
                .ForMember(x => x.Title, y => y.MapFrom(x => x.Post.Title))
                .ForMember(x => x.CommentId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.CommentAuthor, y => y.MapFrom(x => x.Author.UserName))
                .ForMember(x => x.CommentAuthorId, y => y.MapFrom(x => x.AuthorId))
                .ForMember(x => x.Content, y => y.MapFrom(x => x.Content));

            CreateMap<Book, CreateBookViewModel>().ReverseMap();

            CreateMap<Book, BookIndexViewModel>().ReverseMap();

            CreateMap<Post, PostIndexViewModel>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.Author, y => y.MapFrom(x => x.Author.UserName))
                .ForMember(x => x.Title, y => y.MapFrom(x => x.Title))
                .ForMember(x => x.FullContent, y => y.MapFrom(x => x.FullContent))
                .ForMember(x => x.CommentCount, y => y.MapFrom(x => x.Comments.Count()))
                .ForMember(x => x.CreatedOn, y => y.MapFrom(x => x.CreatedOn.ToString("dd-MM-yyyy HH:mm")))
                .ForMember(x => x.IsDeleted, y => y.MapFrom(x => x.IsDeleted))
                .ForMember(x => x.Tags, y => y.MapFrom(x => string.Join(", ", x.Tags.Select(tp => tp.Tag.Name))));

        }
    }
}
