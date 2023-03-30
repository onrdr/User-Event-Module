using AutoMapper;
using Models.Concrete.DTOs;
using Models.Concrete.Entities;

namespace API.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Event, EventDto>().ReverseMap(); 
    }
}
