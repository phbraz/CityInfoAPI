using AutoMapper;
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
    private readonly CityInfoRepository _cityInfoRepository;
    private readonly Mapper _mapper;

    public PointsOfInterestController(
        ILogger<PointsOfInterestController> logger, 
        IMailService mailService, 
        CitiesDataStore citiesDataStore,
        CityInfoRepository cityInfoRepository,
        Mapper mapper)
    {
        _logger =  logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _citiesDataStore = citiesDataStore;
        _cityInfoRepository = cityInfoRepository;
        _mapper = mapper;
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
    public async  Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,
        PointOfInterestForCreationDto pointOfInterest)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

        await _cityInfoRepository.AddPointsOfInterestForCityAsync(cityId, finalPointOfInterest);

        var createdPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);
            
            
        return CreatedAtRoute("GetPointOfInterest", new
        {
            cityId,
            pointOfInterestId = createdPointOfInterestToReturn.Id
        }, createdPointOfInterestToReturn);

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