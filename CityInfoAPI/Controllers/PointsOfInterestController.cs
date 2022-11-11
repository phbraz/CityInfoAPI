using CityInfoAPI.Models;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly CitiesDataStore _citiesDataStore;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore)
    {
        _logger =  logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _citiesDataStore = citiesDataStore;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

        if (city == null)
        {
            _logger.LogInformation($"City with the following cityId: {cityId} was not found");
            return NotFound();
        }

        return Ok(city.PointOfInterest);
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        try
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            
            if (city == null)
            {
                _logger.LogInformation($"City with the following cityId: {cityId} was not found");
                return NotFound();
            }
            return Ok(city.PointOfInterest);

        }
        catch (Exception e)
        {
            _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", e);
            return StatusCode(500, "A problem happened while handling your request.");
        }

    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId,
        PointOfInterestForCreationDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }

        var maxPointOfInterest = _citiesDataStore.Cities.SelectMany(x => x.PointOfInterest).Max(x => x.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterest,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };
        
        city.PointOfInterest.Add(finalPointOfInterest);

        return CreatedAtRoute("GetPointOfInterest", new
        {
            cityId = cityId,
            pointOfInterestId = finalPointOfInterest.Id
        }, finalPointOfInterest);

    }

    [HttpPut("{pointofinterestid}")]
    public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdate = city.PointOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
        
        if (pointOfInterestToUpdate == null)
        {
            return NotFound();
        }

        pointOfInterestToUpdate.Name = pointOfInterest.Name;
        pointOfInterestToUpdate.Description = pointOfInterest.Description;

        return NoContent();
    }

    [HttpPatch("{pointofinterestid}")]
    public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdate = city.PointOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
        
        if (pointOfInterestToUpdate == null)
        {
            return NotFound();
        }

        var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
        {
            Name = pointOfInterestToUpdate.Name,
            Description = pointOfInterestToUpdate.Description
        };
        
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);
        }

        pointOfInterestToUpdate.Name = pointOfInterestToPatch.Name;
        pointOfInterestToUpdate.Description = pointOfInterestToPatch.Description;

        return NoContent();
    }

    [HttpDelete("{pointofinterestid}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToDelete = city.PointOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
        
        if (pointOfInterestToDelete == null)
        {
            return NotFound();
        }

        city.PointOfInterest.Remove(pointOfInterestToDelete);
        _mailService.Send(
            "Points of Interest deleted",
            $"point of interest {pointOfInterestToDelete.Name} with id {pointOfInterestToDelete.Id} has been deleted."
            );

        return NoContent();
    }
}