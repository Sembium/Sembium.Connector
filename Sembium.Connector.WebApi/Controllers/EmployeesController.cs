using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sembium.Connector.Services;
using Microsoft.AspNetCore.Authorization;
using Sembium.Connector.Entities;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Sembium.Connector.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeesService _employeesService;

        public EmployeesController(IEmployeesService employeesService)
        {
            _employeesService = employeesService;
        }

        // GET api/employees/AddEmployeeMovement?employeeNo=123456&inOut=1&movementDateTime=2017-01-12T17:29:47Z
        [Route("AddEmployeeMovement")]
        [HttpPost]
        public void AddEmployeeMovement([FromQuery] int employeeNo, [FromQuery] int inOut, [FromQuery] DateTime movementDateTime)
        {
            _employeesService.AddEmployeeMovement(employeeNo, inOut, movementDateTime);
        }

        // GET api/employees/AddEmployeeMovements
        [Route("AddEmployeeMovements")]
        [HttpPost]
        public void AddEmployeeMovements([FromBody] IEnumerable<EmployeeMovement> movements)
        {
            foreach (var movement in movements)
            {
                _employeesService.AddEmployeeMovement(movement.EmployeeNo, movement.InOut, movement.MovementDateTime);
            }
        }        
    }
}
