using AutoMapper;
using Obtain_25_Cat_Images.DTOs;
using Obtain_25_Cat_Images.Models.Entities;

namespace Obtain_25_Cat_Images.Mappings {
    public class MapperProfile : Profile {

        public MapperProfile() {
            CreateMap<CatEntity, CatResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CatId))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                    src.CatTags.Select(ct => new TagResponseDTO { Name = ct.Tag.Name })));
            CreateMap<TagEntity, TagResponseDTO>();
        }
    }
}
