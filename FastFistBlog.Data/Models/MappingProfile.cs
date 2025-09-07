using AutoMapper;
using FastFistBlog.Data.Models.DTO;

namespace FastFistBlog.Data.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Article, ArticleDto>()
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => src.ArticleTags.Select(at => at.TagId).ToList()))
            .ForMember(dest => dest.Comments,
                opt => opt.MapFrom(src => src.Comments.Select(c => c.Id).ToList()));
        CreateMap<ArticleDto, Article>()
            .ForMember(dest => dest.ArticleTags,
                opt => opt.Ignore())
            .ForMember(dest => dest.Comments,
                opt => opt.Ignore())
            .ForMember(dest => dest.Author,
                opt => opt.Ignore());
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.ArticleId,
                opt => opt.MapFrom(src => src.ArticleId))
            .ForMember(dest => dest.AuthorId,
                opt => opt.MapFrom(src => src.AuthorId));
        CreateMap<CommentDto, Comment>()
            .ForMember(dest => dest.Article,
                opt => opt.Ignore())
            .ForMember(dest => dest.Author,
                opt => opt.Ignore());
        CreateMap<Tag, TagDto>();
        CreateMap<TagDto, Tag>()
            .ForMember(dest => dest.ArticleTags,
                opt => opt.Ignore());
        CreateMap<ApplicationUser, ApplicationUserDto>()
            .ForMember(dest => dest.Articles,
                opt => opt.MapFrom(src => src.Articles.Select(a => a.Id).ToList()))
            .ForMember(dest => dest.Comments,
                opt => opt.MapFrom(src => src.Comments.Select(c => c.Id).ToList()))
            .ForMember(dest => dest.PasswordHash,
                opt => opt.MapFrom(src => src.PasswordHash));
        CreateMap<ApplicationUserDto, ApplicationUser>()
            .ForMember(dest => dest.Articles,
                opt => opt.Ignore())
            .ForMember(dest => dest.Comments,
                opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash,
                opt => opt.Ignore());
        CreateMap<ApplicationRole, ApplicationRoleDto>();
        CreateMap<ApplicationRoleDto, ApplicationRole>()
            .ForMember(dest => dest.NormalizedName, 
            opt => opt.MapFrom(src => src.Name.ToUpper()));
    }
}