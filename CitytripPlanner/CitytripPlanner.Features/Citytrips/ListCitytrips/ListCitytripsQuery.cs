using MediatR;

namespace CitytripPlanner.Features.Citytrips.ListCitytrips;

public record ListCitytripsQuery : IRequest<List<CitytripCard>>;
