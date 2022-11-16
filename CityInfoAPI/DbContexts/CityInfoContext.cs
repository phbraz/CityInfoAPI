using CityInfoAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfoAPI.DbContexts;

public class CityInfoContext : DbContext
{
    public CityInfoContext(DbContextOptions<CityInfoContext> options) 
        : base(options)
    {
    }
    
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<PointOfInterest> PointOfInterests { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new City("Sao Paulo")
            {
                Id = 1,
                Description = "Greatest city in the world but not really"
            },
            new City("Manchester")
            {
                Id = 2,
                Description = "Fish and chips! Home!"
            },
            new City("Paris")
            {
                Id = 3,
                Description = "Stolen art across the globe can be found here"
            }
        );

        modelBuilder.Entity<PointOfInterest>().HasData(
            new PointOfInterest("Avenida Paulista")
            {
                Id = 1,
                CityId = 1,
                Description = "Malls and malls and shops buy buy buy!"
            },
            new PointOfInterest("Ibirapuera Park")
            {
                Id = 2,
                CityId = 1,
                Description = "Finally some green in this concrete jungle"
            },
            new PointOfInterest("Arndale Mall")
            {
                Id = 3,
                CityId = 2,
                Description = "Where Capitalism always win, sadly"
            },
            new PointOfInterest("Neymar")
            {
                Id = 4,
                CityId = 3,
                Description = "Stolen art across the globe can be found here"
            }
        );
        
        base.OnModelCreating(modelBuilder);
    }
}