using AutoMapper;
using ShoeGrabCommonModels;
using ShoeGrabCommonModels.Dto;
using ShoeGrabOrderManagement.Dto;
using ShoeGrabProductManagement.Dto;
using ShoeGrabUserManagement.Models.Dto;

namespace ShoeGrabMonolith.Database.Mappers;

public class GlobalMappingProfile : Profile
{
    public GlobalMappingProfile()
    {
        //User
        CreateMap<UserProfileDto, UserProfile>();
        CreateMap<AddressDto, Address>();
        CreateMap<UserProfile, UserProfileDto>();
        CreateMap<Address, AddressDto>();

        //Order
        CreateMap<Order, OrderDto>();
        CreateMap<OrderDto, Order>();
        CreateMap<CreateOrderDto, Order>();
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<OrderItemCreateDto, OrderItem>();
        CreateMap<OrderItemDto, OrderItem>();
        CreateMap<PaymentInfoDto, PaymentInfo>();
        CreateMap<PaymentInfo, PaymentInfoDto>();

        //Basket
        CreateMap<Basket, BasketDto>();
        CreateMap<BasketItem, BasketItemDto>();
        CreateMap<BasketDto, Basket>();
        CreateMap<BasketItemDto, BasketItem>();

        //Product
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();
    }
}
