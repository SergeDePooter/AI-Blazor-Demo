using MediatR;

namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

public record GetCitytripDetailQuery(int Id) : IRequest<CitytripDetailResponse?>;
