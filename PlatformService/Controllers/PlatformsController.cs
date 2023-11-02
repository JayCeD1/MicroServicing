﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo repo;
        private readonly IMapper mapper;
        private readonly ICommandDataClient commandDataClient;

        public PlatformsController(IPlatformRepo repo, IMapper mapper, ICommandDataClient commandDataClient)
        {
            this.repo = repo;
            this.mapper = mapper;
            this.commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms() 
        {
            Console.WriteLine("--> Getting Platforms");

            var platformItem = repo.GetAllPlatforms();
            
            return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine("--> Getting Platform");

            var platformItem = repo.GetPlatformById(id);
            if (platformItem != null)
            {
                return Ok(mapper.Map<PlatformReadDto>(platformItem));

            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel = mapper.Map<Platform>(platformCreateDto);
            repo.CreatePlatform(platformModel);
            repo.SaveChanges();

            var platformReadDto = mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                await commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"-->Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDto.Id }, platformReadDto);
        }
    }
}
