using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlantManagerAPI.Infrastructure;
using PlantManagerAPI.Infrastructure.Repositories;
using PlantManagerAPI.Infrastructure.Repositories.CompanyRepositories;
using PlantManagerAPI.Models;
using PlantManagerAPI.Models.Company;

namespace PlantManagerAPI.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class PlantsController : ControllerBase
    {
        private readonly PlantManagerContext _context;
        private readonly ILogger<PlantsController> _logger;
        private readonly IGenericRepository<PlantModel> _plantRepository;

        public PlantsController(ILogger<PlantsController> logger, IGenericRepository<PlantModel> plantRepository)
        {
            _logger = logger;
            _plantRepository = plantRepository;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IQueryable<PlantModel>>> GetPlants([FromQuery] string name,[FromQuery] APIRequestSize request)
        {
            var res = await _plantRepository.GetAllAsync();


            Response.Headers["x-total-count"] = res.Count().ToString();

            if (!string.IsNullOrEmpty(name))
            {
                res = res.Where(p => p.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
            }

            if (request.Limit >=100)
            {
                _logger.LogInformation("Requesting more thant 100 plants");
            }
            //return Ok();
            return Ok(res.OrderBy(p => p.PlantId).Skip(request.Offset).Take(request.Limit));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PlantModel> PostPlant([FromBody] PlantModel plant)
        {
            try
            {
                _context.Plants.Add(plant);
                _context.SaveChanges();

                return new CreatedResult($"/plants/{plant.PlantId}", plant);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to POST product");

                return ValidationProblem(e.Message);
            }
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PlantModel> GetPlantById([FromRoute]string id)
        {
            var plantDb = _context.Plants.FirstOrDefault(p => p.PlantId.ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (plantDb == null)
            {
                _logger.LogInformation("Requested plant was not found", id);
                return NotFound();
            }

            return Ok(plantDb);
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PlantModel> PutPlant([FromBody] PlantModel plant)
        {
            try
            {
                var plantDb = _context.Plants.FirstOrDefault(p => p.PlantId.Equals(plant.PlantId));

                if (plantDb==null)
                {
                    _logger.LogInformation("Requested plant was not found", plant);
                    return NotFound();
                }

                plantDb.Name = plant.Name;
                plantDb.Address = plant.Address;
                plantDb.City = plant.City;
                plantDb.Email = plant.Email;
                plantDb.Phone = plant.Phone;

                _context.SaveChanges();

                return Ok(plant);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to PUT plant");

                return ValidationProblem(e.Message);
            }
        }


        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PlantModel> DeletePlant([FromRoute] string id)
        {

            var noPlants = _context.Plants.Count();
            if (noPlants == 1)
            {
                return ValidationProblem("You cannot delete the only item.");
            }

            var plantsDb = _context.Plants.FirstOrDefault(p => p.PlantId.ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (plantsDb==null)
            {
                _logger.LogInformation("Requested plant was not found", id);
                return NotFound();
            }

            _context.Plants.Remove(plantsDb);
            _context.SaveChanges();

            return NoContent();
        }

    }
}
