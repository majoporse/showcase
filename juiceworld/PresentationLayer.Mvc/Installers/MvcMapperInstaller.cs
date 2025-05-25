using AutoMapper;
using BusinessLayer.DTOs;
using Infrastructure.QueryObjects;
using PresentationLayer.Mvc.Areas.Admin.Models;
using PresentationLayer.Mvc.Areas.Customer.Models;
using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Installers;

public class MvcMapperInstaller : Profile
{
    public MvcMapperInstaller()
    {
        CreateMap<UserProfileViewModel, UserDto>().ReverseMap();
        CreateMap<UserSimpleViewModel, UserDto>().ReverseMap();
        CreateMap<UserUpdateRestrictedViewModel, UserDto>().ReverseMap();
        CreateMap<UserUpdateRestrictedViewModel, UserUpdateDto>().ReverseMap();
        CreateMap<UserUpdateViewModel, UserDto>().ReverseMap();
        CreateMap<UserUpdateViewModel, UserUpdateDto>().ReverseMap();

        CreateMap<AddToCartViewModel, AddToCartDto>().ReverseMap();
        CreateMap<BaseEntityViewModel, BaseEntityDto>().ReverseMap();
        CreateMap<CartItemDetailViewModel, CartItemDetailDto>().ReverseMap();
        CreateMap<CouponCodeViewModel, CouponCodeDto>().ReverseMap();
        CreateMap<CreateOrderViewModel, CreateOrderDto>().ReverseMap();

        CreateMap<GiftCardCreateViewModel, GiftCardCreateDto>().ReverseMap();
        CreateMap<GiftCardDetailViewModel, GiftCardDetailDto>().ReverseMap();
        CreateMap<GiftCardEditViewModel, GiftCardEditDto>().ReverseMap();
        CreateMap<GiftCardFilterViewModel, GiftCardFilterDto>().ReverseMap();
        CreateMap<GiftCardViewModel, GiftCardViewDto>().ReverseMap();

        CreateMap<ManufacturerFilterViewModel, ManufacturerFilterDto>().ReverseMap();
        CreateMap<ManufacturerViewModel, ManufacturerDto>().ReverseMap();

        CreateMap<OrderDetailViewModel, OrderDetailDto>().ReverseMap();
        CreateMap<OrderViewModel, OrderDto>().ReverseMap();

        CreateMap<PaginationViewModel, PaginationDto>().ReverseMap();
        CreateMap<ProductDetailViewModel, ProductDetailDto>().ReverseMap();
        CreateMap<ProductImageViewModel, ProductDto>().ReverseMap();
        CreateMap<ProductFilterViewModel, ProductFilterDto>().ReverseMap();
        CreateMap<ProductImageViewModel, ProductImageDto>().ReverseMap();
        CreateMap<ProductViewModel, ProductDto>().ReverseMap();

        CreateMap<ReviewViewModel, ReviewDto>().ReverseMap();
        CreateMap<TagViewModel, TagDto>().ReverseMap();
        CreateMap<UserFilterViewModel, UserFilterDto>().ReverseMap();
        CreateMap<UserRegisterViewModel, UserRegisterDto>().ReverseMap();
        CreateMap<UserUpdateViewModel, UserUpdateDto>().ReverseMap();
        CreateMap<WishlistItemDetailViewModel, WishListItemDetailDto>().ReverseMap();

        CreateMap<SearchablesFilterViewModel, SearchablesFilterDto>().ReverseMap();
        CreateMap<SearchablesFilterResultViewModel, SearchablesFilterResultDto>().ReverseMap();

        CreateMap<SearchablesFilterDto, ProductFilterDto>()
            .ForMember(dest => dest.NameQuery, opt => opt.MapFrom(src => src.NameQuery))
            .ForMember(dest => dest.PriceMax, opt => opt.MapFrom(src => src.ProductPriceMax))
            .ForMember(dest => dest.PriceMin, opt => opt.MapFrom(src => src.ProductPriceMin))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.ProductCategory))
            .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.ProductPageIndex))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.ProductPageSize))
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.TagId))
            .ForMember(dest => dest.ManufacturerId, opt => opt.MapFrom(src => src.ManufacturerId));

        CreateMap<SearchablesFilterDto, ManufacturerFilterDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameQuery))
            .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.ManufacturerPageIndex))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.ManufacturerPageSize));

        CreateMap<SearchablesFilterDto, TagFilterDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameQuery))
            .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.TagPageIndex))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.TagPageSize));

        CreateMap(typeof(FilteredResult<>), typeof(FilteredResult<>)).ConvertUsing(typeof(FilteredResultConverter<,>));
    }
}
