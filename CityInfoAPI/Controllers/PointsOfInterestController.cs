using AutoMapper;
using CityInfoAPI.DbContexts;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityInfoAPI.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    public PointsOfInterestController(
        ILogger<PointsOfInterestController> logger, 
        IMailService mailService,
        ICityInfoRepository cityInfoRepository,
        IMapper mapper)
    {
        _logger =  logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _cityInfoRepository = cityInfoRepository;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            _logger.LogInformation($"City with the following cityId: {cityId} was not found");
            return NotFound();
        }

        var pointsOfInterestEntity = await _cityInfoRepository
            .GetPointsOfInterestForCityAsync(cityId);
        if (pointsOfInterestEntity == null)
        {
            _logger.LogInformation($"No points of interest were found for the following cityId: {cityId}");
            return NotFound();
        }

        return Ok(pointsOfInterestEntity);
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        try
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with the following cityId: {cityId} was not found");
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            
            if (pointOfInterestEntity == null)
            {
                _logger.LogInformation($"No point of interest was found for the following cityId: {cityId}");
                return NotFound();
            }
            
            return Ok(pointOfInterestEntity);
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
    public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterest)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }
    
        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }
    
        _mapper.Map(pointOfInterest, pointOfInterestEntity);
    
        await _cityInfoRepository.SaveChangesAsync();
    
        return NoContent();
    }

    [HttpPatch("{pointofinterestid}")]
    public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }
        
        var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);
        
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
    
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
    
        if (!TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{pointofinterestid}")]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }
        
        _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();
        
        _mailService.Send(
            "Points of Interest deleted",
            $"point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} has been deleted."
            );
    
        return NoContent();
    }
}