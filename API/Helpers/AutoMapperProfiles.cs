using AutoMapper;
using API.Entities;
using API.Dtos;
using System.Linq;
using API.extensions;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser , MemberDto>()
                    .ForMember(dest => dest.Age , opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                    .ForMember(dest => dest.PhotoUrl , 
                      opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url 
                      ));
            CreateMap<Photo , PhotoDto>();
            CreateMap<MemberUpdateDto , AppUser>();
            CreateMap<RegisterDto , AppUser>();
            CreateMap<AppUser , LikeDto>()
                    .ForMember(dest => dest.Age , 
                              opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                    .ForMember(dest => dest.PhotoUrl , 
                              opt => opt.MapFrom(src => src.Photos.SingleOrDefault(x => x.IsMain).Url));
            CreateMap<Message , MessageDto>()
                    .ForMember(dest => dest.SenderPhotoUrl , 
                               opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(i => i.IsMain).Url)
                              )
                    .ForMember(dest => dest.RecipientPhotoUrl , 
                               opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(i => i.IsMain).Url)
                              );
        }
    }
}