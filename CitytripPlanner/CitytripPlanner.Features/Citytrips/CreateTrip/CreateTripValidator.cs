namespace CitytripPlanner.Features.Citytrips.CreateTrip;

public class CreateTripValidator
{
    public List<string> Validate(CreateTripCommand command)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(command.Title))
            errors.Add("Title is required.");
        else if (command.Title.Length > 100)
            errors.Add("Title must be 100 characters or less.");

        if (string.IsNullOrWhiteSpace(command.Destination))
            errors.Add("Destination is required.");
        else if (command.Destination.Length > 100)
            errors.Add("Destination must be 100 characters or less.");

        if (command.EndDate < command.StartDate)
            errors.Add("End date must be on or after start date.");

        if (command.MaxParticipants.HasValue && command.MaxParticipants.Value < 1)
            errors.Add("Max participants must be at least 1.");

        if (command.ImageUrl is not null &&
            !Uri.TryCreate(command.ImageUrl, UriKind.Absolute, out _))
            errors.Add("Image URL must be a valid URL.");

        if (command.DayPlans is not null)
        {
            for (int di = 0; di < command.DayPlans.Count; di++)
            {
                var dp = command.DayPlans[di];

                if (dp.Date < command.StartDate || dp.Date > command.EndDate)
                    errors.Add($"Day {dp.DayNumber} date is outside the trip date range.");

                for (int ei = 0; ei < dp.Events.Count; ei++)
                {
                    var ev = dp.Events[ei];

                    if (ev.EndTime.HasValue && ev.EndTime.Value <= ev.StartTime)
                        errors.Add($"End time must be after start time for day {dp.DayNumber}, event {ei + 1}.");

                    if (ev.Place is not null)
                    {
                        if (ev.Place.Latitude < -90 || ev.Place.Latitude > 90)
                            errors.Add($"Latitude is out of range for day {dp.DayNumber}, event {ei + 1}.");

                        if (ev.Place.Longitude < -180 || ev.Place.Longitude > 180)
                            errors.Add($"Longitude is out of range for day {dp.DayNumber}, event {ei + 1}.");
                    }
                }
            }
        }

        return errors;
    }
}
