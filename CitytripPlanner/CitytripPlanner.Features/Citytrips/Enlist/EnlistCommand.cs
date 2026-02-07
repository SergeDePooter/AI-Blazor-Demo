using MediatR;

namespace CitytripPlanner.Features.Citytrips.Enlist;

public record EnlistCommand(int CitytripId) : IRequest<bool>;
