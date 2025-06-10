using Events.Application.Commands;
using Events.Domain.Repositories;
using FluentValidation;

namespace Events.Application.Validators
{
    public class DeleteEventImageCommandValidator
        : AbstractValidator<DeleteEventImageCommand>
    {
        public DeleteEventImageCommandValidator(
            IEventRepository eventRepo,
            IEventImageRepository imageRepo)
        {
            RuleFor(x => x.ImageId)
                .NotEmpty().WithMessage("ImageId is required.");
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");

            RuleFor(cmd => cmd.EventId)
                .MustAsync(async (eventId, ct) =>
                    await eventRepo.GetByIdAsync(eventId, ct)!=null)
                .WithMessage(cmd => $"Event {cmd.EventId} not found.");

            RuleFor(x => x)
                .MustAsync(async (cmd, ct) =>
                {
                    var img = await imageRepo
                        .GetByIdAsync(cmd.ImageId, ct);
                    return img != null
                        && img.EventId == cmd.EventId;
                })
                .WithMessage(cmd =>
                    $"Image {cmd.ImageId} does not belong to Event {cmd.EventId}.");
        }
    }
}
