using DBServer;
using DBServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindSample.Controller
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private DataServer _dataServer;
        public EmployeesController(DataServer dataServer)
        {
            _dataServer = dataServer;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var allEmployees = await _dataServer.GetAllEmployee();

            //present cycle when serialize
            allEmployees.ToList().ForEach(employee => employee.InverseReportsToNavigation = null);

            if (allEmployees != null)
                return Ok(allEmployees);
            else
                return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Employee employee)
        {
            return Ok(await _dataServer.InsertOrUpdateEmployee(employee));
        }
        [HttpPost]
        public async Task<IActionResult> Update(Employee employee)
        {
            return Ok(await _dataServer.InsertOrUpdateEmployee(employee));
        }
        [HttpDelete("{employeeID}")]
        public async Task<IActionResult> Delete(int employeeID)
        {
            return Ok(await _dataServer.DeleteEmplyee(employeeID));
        }


    }
}
