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
    public class MachinesController : ControllerBase
    {
        private readonly PlantManagerContext _context;
        private readonly ILogger<MachinesController> _logger;

        public MachinesController(PlantManagerContext context, ILogger<MachinesController> logger)
        {
            _context = context;
            _logger = logger;
            if (_context.Machines.Any())
            {
                return;
            }
            PlantSeed.InitData(context);
        }


        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IQueryable<Machine>> GetMachines([FromQuery] string name, [FromQuery] APIRequestSize requestSize)
        {
            var result = _context.Machines as IQueryable<Machine>;

            Response.Headers["x-total-count"] = result.Count().ToString();

            if (!string.IsNullOrEmpty(name))
            {
                result = result.Where(m => m.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
            }

            if (requestSize.Limit >= 100)
            {
                _logger.LogInformation("Requesting more thant 100 cost centers");
            }

            return Ok(result
              .OrderBy(p => p.Name).Skip(requestSize.Offset).Take(requestSize.Limit));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Machine> PostMachine([FromBody] Machine machine)
        {
            try
            {
                var machineDb = _context.Machines.FirstOrDefault(m => m.MachineId.Equals(machine.MachineId));

                if (machineDb == null)
                {
                    _logger.LogInformation("Requested MachineId :{machine.MachineId} was not found", machine.MachineId);
                    return NotFound();
                }

                _context.Machines.Add(machine);
                _context.SaveChanges();

                return new CreatedResult($"/plants/{machine.MachineId}", machine);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to POST machine");

                return ValidationProblem(e.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CostCenter> GetMachineById([FromRoute] string id)
        {
            var machineDb = _context.Machines.FirstOrDefault(m => m.MachineId.ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (machineDb == null)
            {
                _logger.LogInformation("Requested machineId: {id} was not found", id);

                return NotFound();
            }

            return Ok(machineDb);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Machine> PutMachine([FromBody] Machine machine)
        {
            try
            {
                var machineDb = _context.Machines.FirstOrDefault(m => m.MachineId.Equals(machine.MachineId));

                if (machineDb == null)
                {
                    _logger.LogInformation("Requested machine : {costCenter} was not found", machine);
                    return NotFound();
                }

                var costCenterDb = _context.CostCenters.FirstOrDefault(c => c.CostCenterId.Equals(machine.CostCenterId));

                if (costCenterDb == null)
                {
                    _logger.LogInformation("Requested CostCenter:{machine.CostCenterId} was not found", machine.CostCenterId);
                    return NotFound();
                }

                machineDb.CostCenterId = machine.CostCenterId;

                machineDb.Name = machine.Name;
                machineDb.Description = machine.Description;
                machineDb.PartsPerCycle = machine.PartsPerCycle;
                machineDb.ProcessTime = machine.ProcessTime;
                machineDb.RatePerHour = machine.RatePerHour;
                machineDb.SetupTime = machine.SetupTime;
                machineDb.PartsPerCycle = machine.PartsPerCycle;

                machineDb.AlarmOnOff = machine.AlarmOnOff;
                machineDb.AlarmDate = machine.AlarmOnOff ? DateTime.Now : default;
                machineDb.ModifiedDate = DateTime.Now;

                _context.SaveChanges();

                return Ok(machine);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to PUT machine");

                return ValidationProblem(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PlantModel> DeleteMachine([FromRoute] string id)
        {
            var noMachines = _context.Machines.Count();
            if (noMachines == 1)
            {
                return ValidationProblem("You cannot delete the only item.");
            }

            var machineDb = _context.Machines.FirstOrDefault(m => m.MachineId.ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (machineDb == null)
            {
                _logger.LogInformation("Requested machine:{id} was not found", id);
                return NotFound();
            }

            _context.Machines.Remove(machineDb);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
