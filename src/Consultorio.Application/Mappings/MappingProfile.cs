using AutoMapper;
using Consultorio.Domain.Entities;
using Consultorio.Application.Dtos.Paciente;

namespace Consultorio.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreatePacienteDto, Paciente>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForMember(dest => dest.Agendamentos, opt => opt.Ignore())
                .ForMember(dest => dest.Historicos, opt => opt.Ignore());

            CreateMap<UpdatePacienteDto, Paciente>()
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => System.DateTime.Now))
                .ForMember(dest => dest.Agendamentos, opt => opt.Ignore())
                .ForMember(dest => dest.Historicos, opt => opt.Ignore());

            CreateMap<Paciente, PacienteResponseDto>();
        }
    }
}
