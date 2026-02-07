using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.Enlist;

public class EnlistHandler(
    ICitytripRepository repository,
    IUserInteractionStore interactionStore)
    : IRequestHandler<EnlistCommand, bool>
{
    public async Task<bool> Handle(
        EnlistCommand request, CancellationToken cancellationToken)
    {
        var trip = await repository.GetByIdAsync(request.CitytripId);
        if (trip is null)
            throw new ArgumentException($"Citytrip {request.CitytripId} not found.");

        var interaction = interactionStore.GetInteraction(request.CitytripId);
        interaction.IsEnlisted = true;
        interactionStore.SaveInteraction(request.CitytripId, interaction);

        return interaction.IsEnlisted;
    }
}
