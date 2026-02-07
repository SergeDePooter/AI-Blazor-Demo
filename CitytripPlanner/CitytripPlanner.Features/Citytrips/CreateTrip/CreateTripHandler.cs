using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.CreateTrip;

public class CreateTripHandler(ICitytripRepository repository)
    : IRequestHandler<CreateTripCommand, int>
{
    public async Task<int> Handle(
        CreateTripCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateTripValidator();
        var errors = validator.Validate(request);
        if (errors.Count > 0)
            throw new ArgumentException(string.Join(" ", errors));

        var trip = new Citytrip(
            0,
            request.Title,
            request.Destination,
            "",
            request.StartDate,
            request.EndDate,
            request.CreatorId,
            request.Description,
            request.MaxParticipants);

        var created = await repository.AddAsync(trip);
        return created.Id;
    }
}
