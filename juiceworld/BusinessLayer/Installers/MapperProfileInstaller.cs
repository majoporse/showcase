using AutoMapper;
using BusinessLayer.DTOs;
using JuiceWorld.Entities;

namespace BusinessLayer.Installers;

public class MapperProfileInstaller : Profile
{
    public MapperProfileInstaller()
    {
        CreateMap<CartItemDto, CartItem>().ReverseMap();
        CreateMap<CartItemDetailDto, CartItem>().ReverseMap();
        CreateMap<ManufacturerDto, Manufacturer>().ReverseMap();
        CreateMap<OrderDto, Order>().ReverseMap();
        CreateMap<OrderDetailDto, Order>().ReverseMap();
        CreateMap<CreateOrderDto, Order>().ReverseMap();
        CreateMap<OrderProductDto, OrderProduct>().ReverseMap();
        CreateMap<OrderProductDetailDto, OrderProduct>().ReverseMap();
        CreateMap<ProductDto, Product>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.TagIds.Select(id => new Tag { Id = id })))
            .ReverseMap()
            .ForMember(dest => dest.TagIds, opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Id)));
        CreateMap<ProductDetailDto, Product>().ReverseMap();
        CreateMap<ReviewDto, Review>().ReverseMap();
        CreateMap<ReviewDetailDto, Review>().ReverseMap();
        CreateMap<UserDto, User>().ReverseMap();
        CreateMap<UserDto, UserUpdateDto>().ReverseMap();
        CreateMap<User, UserUpdateDto>().ReverseMap();
        CreateMap<UserDto, UserRegisterDto>().ReverseMap();
        CreateMap<UserRegisterDto, UserDto>().ReverseMap();
        CreateMap<WishListItemDto, WishListItem>().ReverseMap();
        CreateMap<WishListItemDetailDto, WishListItem>().ReverseMap();
        CreateMap<AuditTrailDto, AuditTrail>().ReverseMap();
        CreateMap<GiftCardCreateDto, GiftCard>().ReverseMap();
        CreateMap<GiftCardDetailDto, GiftCard>().ReverseMap();
        CreateMap<CouponCodeDto, CouponCode>().ReverseMap();
        CreateMap<GiftCardEditDto, GiftCard>().ReverseMap();
        CreateMap<TagDto, Tag>().ReverseMap();
        CreateMap<ProductDto, ProductImageDto>().ReverseMap();
    }
}