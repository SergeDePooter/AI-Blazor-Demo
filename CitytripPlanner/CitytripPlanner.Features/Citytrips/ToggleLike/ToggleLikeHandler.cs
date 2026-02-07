using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.ToggleLike;

public class ToggleLikeHandler(
    ICitytripRepository repository,
    IUserInteractionStore interactionStore)
    : IRequestHandler<ToggleLikeCommand, bool>
{
    public async Task<bool> Handle(
        ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        var trip = await repository.GetByIdAsync(request.CitytripId);
        if (trip is null)
            throw new ArgumentException($"Citytrip {request.CitytripId} not found.");

        var interaction = interactionStore.GetInteraction(request.CitytripId);
        interaction.IsLiked = !interaction.IsLiked;
        interactionStore.SaveInteraction(request.CitytripId, interaction);

        return interaction.IsLiked;
    }
}
