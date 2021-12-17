using AutoMapper;
using SubWatch.Common.Models;
using SubWatch.Common.Request;

namespace SubWatch.Services.Mappers
{
    public class MapSubscriptionRequestDtoToSubscription : Profile
    {
        public MapSubscriptionRequestDtoToSubscription()
        {
            CreateMap<SubscriptionRequestDto, Subscription>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.SubscriptionType, opt => opt.MapFrom(src => src.SubscriptionType))
                .ForMember(dest => dest.RenewalCost, opt => opt.MapFrom(src => src.RenewalCost))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.RenewalFrequency, opt => opt.MapFrom(src => src.RenewalFrequency));
        }
    }
}
