using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.UpdateTrip;

public class UpdateTripHandler(ICitytripRepository repository)
    : IRequestHandler<UpdateTripCommand, bool>
{
    public async Task<bool> Handle(
        UpdateTripCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateTripValidator();
        var errors = validator.Validate(request);
        if (errors.Count > 0)
            throw new ArgumentException(string.Join(" ", errors));

        var existing = await repository.GetByIdAsync(request.TripId);
        if (existing is null)
            throw new ArgumentException($"Citytrip {request.TripId} not found.");

        if (existing.CreatorId != request.UserId)
            throw new UnauthorizedAccessException("You can only edit your own citytrips.");

        var updated = existing with
        {
            Title = request.Title,
            Destination = request.Destination,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description,
            MaxParticipants = request.MaxParticipants
        };

        await repository.UpdateAsync(updated);
        return true;
    }
}
