﻿using CityInfoAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly CitiesDataStore _citiesDataStore;

    public CitiesController(CitiesDataStore citiesDataStore)
    {
        _citiesDataStore = citiesDataStore;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {
        return Ok(_citiesDataStore.Cities);
    }
    
    [HttpGet("{id}")]
    public ActionResult<CityDto> GetCity(int id)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
        
        if (city == null)
        {
            return NotFound();
        }

        return Ok(city);
    }
}