namespace CitytripPlanner.Features.Citytrips.UpdateTrip;

public class UpdateTripValidator
{
    public List<string> Validate(UpdateTripCommand command)
    {
        var errors = new List<string>();

        if (command.TripId <= 0)
            errors.Add("Trip ID is required.");

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

        if (!string.IsNullOrEmpty(command.ImageUrl) &&
            !Uri.TryCreate(command.ImageUrl, UriKind.Absolute, out _))
            errors.Add("Image URL must be a valid URL.");

        if (command.DayPlans is not null)
        {
            foreach (var day in command.DayPlans)
            {
                foreach (var ev in day.Events)
                {
                    if (ev.EndTime.HasValue && ev.EndTime.Value <= ev.StartTime)
                        errors.Add("End time must be after start time.");
                }
            }
        }

        return errors;
    }
}
