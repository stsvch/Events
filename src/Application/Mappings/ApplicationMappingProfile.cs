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
            CreateMap<Event, EventDto>();
            CreateMap<Event, EventDetailDto>()
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images))
                .ForMember(d => d.RegisteredCount, o => o.MapFrom(s => s.Participants.Count));

            CreateMap<EventImage, EventImageDto>();

            CreateMap<Participant, ParticipantDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name.ToString()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));

            CreateMap<Category, CategoryDto>();

            CreateMap<CreateEventCommand, Event>()
                .ConstructUsing(cmd => new Event(
                    Guid.NewGuid(),
                    cmd.Title,
                    cmd.Description,   
                    cmd.Date,
                    cmd.Venue,
                    cmd.CategoryId,
                    cmd.Capacity));


            CreateMap<AddEventImageCommand, EventImage>()
                .ConstructUsing(cmd => new EventImage(
                    cmd.EventId,
                    cmd.Url,
                    DateTimeOffset.UtcNow));

            CreateMap<RegisterParticipantCommand, Participant>()
                            .ConstructUsing(cmd =>
                                new Participant(
                                    new PersonName(
                                        cmd.FullName.Contains(" ")
                                            ? cmd.FullName.Substring(0, cmd.FullName.IndexOf(' '))
                                            : cmd.FullName,
                                        cmd.FullName.Contains(" ")
                                            ? cmd.FullName.Substring(cmd.FullName.IndexOf(' ') + 1)
                                            : string.Empty
                                    ),
                                    new EmailAddress(cmd.Email),
                                    cmd.DateOfBirth,
                                    cmd.UserId   
                                )
                            );
        }
    }
}
