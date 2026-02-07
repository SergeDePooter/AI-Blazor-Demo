namespace CitytripPlanner.Features.Citytrips.Domain;

public interface ICitytripRepository
{
    Task<List<Citytrip>> GetAllAsync();
    Task<Citytrip?> GetByIdAsync(int id);
}
