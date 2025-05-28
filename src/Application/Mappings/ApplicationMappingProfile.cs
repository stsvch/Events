using AutoMapper;
using Events.Application.Commands;
using Events.Application.DTOs;
using Events.Domain.Entities;
using Events.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace Events.Application.Mappings
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Event, EventDto>()
                .ForMember(d => d.Availability,
                           opt => opt.MapFrom(src =>
                               src.Participants.Count < src.Capacity
                                   ? "there are spots"
                                   : "no spots"));

            CreateMap<Event, EventDetailDto>()
                .ForMember(d => d.ParticipantCount,
                           opt => opt.MapFrom(src => src.Participants.Count));

            CreateMap<bool, RegistrationStatusDto>()
                .ConvertUsing(src => new RegistrationStatusDto { IsRegistered = src });

            CreateMap<EventImage, EventImageDto>();

            CreateMap<Participant, ParticipantDto>()
                .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src => src.Name.ToString()))
                .ForMember(dest => dest.Email,
                           opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.DateOfBirth,
                           opt => opt.MapFrom(src => src.DateOfBirth));

            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryCommand, Category>()
                .ConstructUsing(cmd => new Category(cmd.Name));

            CreateMap<AddEventImageCommand, EventImage>()
                .ConstructUsing(cmd => new EventImage(
                    cmd.EventId,
                    cmd.Url,
                    DateTimeOffset.UtcNow));

            CreateMap<CreateParticipantProfileCommand, Participant>()
                .ConstructUsing(cmd => new Participant(
                    new PersonName(cmd.FirstName, cmd.LastName),
                    new EmailAddress(cmd.Email),
                    cmd.DateOfBirth,
                    cmd.UserId));

            CreateMap<CreateEventCommand, Event>()
                            .ConstructUsing(cmd => new Event(
                                Guid.NewGuid(),
                                cmd.Title,
                                cmd.Description,
                                cmd.Date,
                                cmd.Venue,
                                cmd.CategoryId,
                                cmd.Capacity));
        }
    }
}
