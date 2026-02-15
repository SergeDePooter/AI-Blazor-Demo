namespace CitytripPlanner.Features.Citytrips.Domain;

public interface ICitytripRepository
{
    Task<List<Citytrip>> GetAllAsync();
    Task<Citytrip?> GetByIdAsync(int id);
    Task<Citytrip?> GetByIdWithItineraryAsync(int id);
    Task<Citytrip> AddAsync(Citytrip trip);
    Task<Citytrip> UpdateAsync(Citytrip trip);
    Task<bool> DeleteAsync(int id);
    Task<List<Citytrip>> GetByCreatorAsync(string creatorId);
}
