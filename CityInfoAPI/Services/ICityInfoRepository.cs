using CityInfoAPI.Entities;

namespace CityInfoAPI.Services;

public interface ICityInfoRepository
{
    Task<IEnumerable<City>> GetCitiesAsync();
    Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
    Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);

    Task AddPointsOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

    Task<bool> SaveChangesAsync();
}