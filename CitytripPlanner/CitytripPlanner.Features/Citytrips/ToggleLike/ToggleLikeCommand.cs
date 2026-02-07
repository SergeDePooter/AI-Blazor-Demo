using MediatR;

namespace CitytripPlanner.Features.Citytrips.ToggleLike;

public record ToggleLikeCommand(int CitytripId) : IRequest<bool>;
