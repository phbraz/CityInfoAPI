using CityInfoAPI.DbContexts;
using CityInfoAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CityInfoAPI.Services;

public class CityInfoRepository : ICityInfoRepository
{
    private readonly CityInfoContext _context;

    public CityInfoRepository(CityInfoContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
    {

        //collection to start from
        var collection = _context.Cities as IQueryable<City>;

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            collection = collection.Where(x => x.Name == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(x => x.Name.Contains(searchQuery)
                                               || (x.Description != null && x.Description.Contains(searchQuery)));
        }

        var totalItemCount = await collection.CountAsync();

        var paginationMetaData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);
        
        var collectionToReturn = await collection
            .OrderBy(x => x.Name)
            .Skip(pageSize * (pageNumber -1))
            .Take(pageSize)
            .ToListAsync();

        //paging needs to handled by last because we need to mind the whole data result
        // then let it calculates the pages.
        return (collectionToReturn, paginationMetaData);
    }

    public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
    {
        if (includePointsOfInterest)
        {
            return await _context.Cities.Include(c => c.PointsOfInterests).Where(x => x.Id == cityId)
                .FirstOrDefaultAsync();
        }

        return await _context.Cities.FirstOrDefaultAsync(x => x.Id == cityId);
    }
    
    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
    {
        return await _context.PointOfInterests.Where(x => x.CityId == cityId).ToListAsync();
    }
    
    public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
    {
        return await _context.PointOfInterests.FirstOrDefaultAsync(x =>
            x.CityId == cityId && x.Id == pointOfInterestId);
    }

    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await _context.Cities.AnyAsync(x => x.Id == cityId);
    }

    public async Task AddPointsOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);
        if (city != null)
        {
            city.PointsOfInterests.Add(pointOfInterest);
        }
    }

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        _context.Remove(pointOfInterest);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() >= 0);
    }
}