using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.DeleteTrip;

public class DeleteTripHandler(ICitytripRepository repository)
    : IRequestHandler<DeleteTripCommand, bool>
{
    public async Task<bool> Handle(
        DeleteTripCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.TripId);
        if (existing is null)
            throw new ArgumentException($"Citytrip {request.TripId} not found.");

        if (existing.CreatorId != request.UserId)
            throw new UnauthorizedAccessException("You can only delete your own citytrips.");

        return await repository.DeleteAsync(request.TripId);
    }
}
