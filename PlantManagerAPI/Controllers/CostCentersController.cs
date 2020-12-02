using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using PlantManagerAPI.Infrastructure;
using PlantManagerAPI.Models;
using PlantManagerAPI.Models.Company;

namespace PlantManagerAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class CostCentersController : ControllerBase
    {
        private readonly PlantManagerContext _context;
        private readonly ILogger<CostCentersController> _logger;

        public CostCentersController(PlantManagerContext context, ILogger<CostCentersController> logger)
        {
            _context = context;
            _logger = logger;
            if (_context.CostCenters.Any())
            {
                return;
            }
            PlantSeed.InitData(context);
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IQueryable<CostCenter>> GetCostCenters([FromQuery] string name,[FromQuery] APIRequestSize request)
        {
            var result = _context.CostCenters as IQueryable<CostCenter>;

            Response.Headers["x-total-count"] = result.Count().ToString();

            if (!string.IsNullOrEmpty(name))
            {
                result = result.Where(c => c.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
            }

            if (request.Limit >= 100)
            {
                _logger.LogInformation("Requesting more thant 100 cost centers");
            }

            return Ok(result.OrderBy(c => c.Name).Skip(request.Offset).Take(request.Limit));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CostCenter> PostCostCenter([FromBody] CostCenter costCenter)
        {
            try
            {
                var departmentDb = _context.Departments.FirstOrDefault(d => d.DepartmentId.Equals(costCenter.DepartmentId));

                if (departmentDb == null)
                {
                    _logger.LogInformation("Requested DepartmentId :{costCenter.DepartmentId} was not found", costCenter.DepartmentId);
                    return NotFound();
                }

                _context.CostCenters.Add(costCenter);
                _context.SaveChanges();

                return new CreatedResult($"/plants/{costCenter.CostCenterId}", costCenter);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to POST costCenter");

                return ValidationProblem(e.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CostCenter> GetCostCenterById([FromRoute] string id)
        {
            var costCenterDb = _context.CostCenters.FirstOrDefault(c => c.CostCenterId.ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (costCenterDb == null)
            {
                _logger.LogInformation("Requested costCenter id: {id} was not found", id);

                return NotFound();
            }

            return Ok(costCenterDb);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CostCenter> PutCostCenter([FromBody] CostCenter costCenter)
        {
            try
            {
                var costCenterDb = _context.CostCenters.FirstOrDefault(c => c.CostCenterId.Equals(costCenter.CostCenterId));

                if (costCenterDb == null)
                {
                    _logger.LogInformation("Requested costCenter : {costCenter} was not found", costCenter);
                    return NotFound();
                }

                var departmentDb = _context.Departments.FirstOrDefault(d => d.DepartmentId.Equals(costCenter.DepartmentId));

                if (departmentDb == null)
                {
                    _logger.LogInformation("Requested DepartmentId:{costCenter.DepartmentId} was not found", costCenter.DepartmentId);
                    return NotFound();
                }

                costCenterDb.Name = costCenter.Name;
                costCenterDb.Description = costCenter.Description;
                costCenterDb.Cost = costCenter.Cost;
                costCenterDb.AverageCost = costCenter.AverageCost;
                costCenterDb.ModifiedDate = DateTime.Now;

                costCenterDb.DepartmentId = costCenter.DepartmentId;

                _context.SaveChanges();

                return Ok(costCenter);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to PUT costCenter");

                return ValidationProblem(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PlantModel> DeletecostCenter([FromRoute] string id)
        {
            var noCostCenter = _context.CostCenters.Count();
            if (noCostCenter == 1)
            {
                return ValidationProblem("You cannot delete the only item.");
            }

            var costCenterDb = _context.CostCenters.FirstOrDefault(c => c.CostCenterId.ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (costCenterDb == null)
            {
                _logger.LogInformation("Requested costCenter:{id} was not found", id);
                return NotFound();
            }

            _context.CostCenters.Remove(costCenterDb);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
