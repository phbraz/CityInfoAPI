using CityInfoAPI.Models;

namespace CityInfoAPI;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }
    //public static CitiesDataStore Current { get; } = new CitiesDataStore();

    public CitiesDataStore()
    {
        Cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id = 1,
                Name = "Sao Paulo",
                Description = "Greatest city in the world but not really",
                PointOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Avenida Paulista",
                        Description = "Malls and malls and shops buy buy buy!"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2,
                        Name = "Ibirapuera Park",
                        Description = "Finally some green in this concrete jungle"
                    }
                }
            },
            new CityDto()
            {
                Id = 2,
                Name = "Manchester",
                Description = "Fish and chips! Home!",
                PointOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 3,
                        Name = "Arndale Mall",
                        Description = "Where Capitalism always win, sadly"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 4,
                        Name = "Football Museum",
                        Description = "Here you can see Pele"
                    }
                }
            },
            new CityDto()
            {
                Id = 3,
                Name = "Paris",
                Description = "Frog eater and a funny looking tower",
                PointOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 5,
                        Name = "Neymar",
                        Description = "You can see Neymar playing for PSG"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 6,
                        Name = "Art Museum",
                        Description = "Stolen art across the globe can be found here"
                    }
                }
            }
        };
    }


}