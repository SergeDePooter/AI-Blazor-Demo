namespace CitytripPlanner.Features.Citytrips.ListCitytrips;

public record CitytripFilterCriteria(
    string? SearchText = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null);

public static class CitytripFilter
{
    public static List<CitytripCard> Apply(
        List<CitytripCard> trips,
        CitytripFilterCriteria criteria)
    {
        var result = trips.AsEnumerable();

        var searchText = criteria.SearchText?.Trim();
        if (!string.IsNullOrEmpty(searchText))
        {
            result = result.Where(t =>
                t.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                t.Destination.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        if (criteria.FromDate is { } fromDate)
        {
            result = result.Where(t => t.EndDate >= fromDate);
        }

        if (criteria.ToDate is { } toDate)
        {
            result = result.Where(t => t.StartDate <= toDate);
        }

        return result.ToList();
    }
}
