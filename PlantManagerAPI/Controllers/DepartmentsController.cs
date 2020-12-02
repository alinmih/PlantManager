using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class DepartmentsController : ControllerBase
    {
        private readonly PlantManagerContext _context;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(PlantManagerContext context, ILogger<DepartmentsController> logger)
        {
            _context = context;
            _logger = logger;
            if (_context.Departments.Any())
            {
                return;
            }
            PlantSeed.InitData(context);
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IQueryable<Department>> GetDepartments([FromQuery] string name,[FromQuery] APIRequestSize request)
        {
            var result = _context.Departments as IQueryable<Department>;

            Response.Headers["x-total-count"] = result.Count().ToString();

            if (!string.IsNullOrEmpty(name))
            {
                result = result.Where(d => d.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
            }

            if (request.Limit >= 100)
            {
                _logger.LogInformation("Requesting more thant 100 departments");
            }

            return Ok(result.OrderBy(d => d.DepartmentId).Skip(request.Offset).Take(request.Limit));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Department> PostDepartment([FromBody] Department department)
        {
            try
            {
                var plantDb = _context.Plants.FirstOrDefault(p => p.PlantId.Equals(department.PlantId));

                if (plantDb == null)
                {
                    _logger.LogInformation("Requested PlantId :{department.PlantId} was not found", department.PlantId);
                    return NotFound();
                }

                _context.Departments.Add(department);
                _context.SaveChanges();

                return new CreatedResult($"/plants/{department.DepartmentId}", department);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to POST department");

                return ValidationProblem(e.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Department> GetDepartmentById([FromRoute] string id)
        {
            var departmentDb = _context.Departments.FirstOrDefault(d => d.DepartmentId.ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (departmentDb == null)
            {
                _logger.LogInformation("Requested department id: {id} was not found", id);

                return NotFound();
            }

            return Ok(departmentDb);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Department> PutDepartment([FromBody] Department department)
        {
            try
            {
                var departmentDb = _context.Departments.FirstOrDefault(d => d.DepartmentId.Equals(department.DepartmentId));

                if (departmentDb == null)
                {
                    _logger.LogInformation("Requested department : {department} was not found", department);
                    return NotFound();
                }

                departmentDb.Name = department.Name;
                departmentDb.Description = department.Description;

                var plantDb = _context.Plants.FirstOrDefault(p => p.PlantId.Equals(department.PlantId));

                if (plantDb == null)
                {
                    _logger.LogInformation("Requested PlantId:{department.PlantId} was not found", department.PlantId);
                    return NotFound();
                }

                departmentDb.PlantId = department.PlantId;

                _context.SaveChanges();

                return Ok(department);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to PUT department");

                return ValidationProblem(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Plant> DeleteDepartment([FromRoute] string id)
        {
            var noDepartments = _context.Departments.Count();
            if (noDepartments == 1)
            {
                return ValidationProblem("You cannot delete the only item.");
            }

            var departmentDb = _context.Departments.FirstOrDefault(d => d.DepartmentId.ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (departmentDb == null)
            {
                _logger.LogInformation("Requested department{id} was not found", id);
                return NotFound();
            }

            _context.Departments.Remove(departmentDb);
            _context.SaveChanges();

            return NoContent();
        }

    }
}
