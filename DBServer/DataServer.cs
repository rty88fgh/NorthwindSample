using AutoMapper;
using DBServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer
{
    public class DataServer
    {
        private IServiceProvider _provider;
        private ILogger<DataServer> _logger;
        private IMapper _mapper;
        public DataServer(IServiceProvider provider, ILogger<DataServer> logger, IMapper mapper)
        {
            _provider = provider;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployee()
        {
            using var db = _provider.CreateScope().ServiceProvider.GetService<DataServerContext>();
            try
            {
                var allEmployees = await db.Employees.ToListAsync();
                return allEmployees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting employee");
                return null;
            }
        }

        public async Task<bool> InsertOrUpdateEmployee(Employee editingEmployee)
        {
            if (editingEmployee == null)
            {
                _logger.LogError("Failed to get employee to insert/update ");
                return false;
            }

            if (string.IsNullOrEmpty(editingEmployee.FirstName) || string.IsNullOrEmpty(editingEmployee.LastName))
            {
                _logger.LogError("Please enter first name and last name");
                return false;
            }

            using var db = _provider.CreateScope().ServiceProvider.GetService<DataServerContext>();
            try
            {
                var origin = await db.Employees.FirstOrDefaultAsync(employee => employee.EmployeeId == editingEmployee.EmployeeId);
                if (origin == null)
                {
                    origin = editingEmployee;
                    await db.Employees.AddAsync(editingEmployee);
                }
                else
                    _mapper.Map(editingEmployee, origin);

                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while interting or updating employee");
                return false;
            }
        }

        public async Task<bool> DeleteEmplyee(int employeeId)
        {

            using var db = _provider.CreateScope().ServiceProvider.GetService<DataServerContext>();
            try
            {
                var origin = await db.Employees.Include(employee => employee.Orders)
                                               .Include(employee => employee.EmployeeTerritories)
                                               .Include(employee => employee.InverseReportsToNavigation)
                                               .FirstOrDefaultAsync(employee => employee.EmployeeId == employeeId);
                if (origin == null)
                {
                    _logger.LogError("Failed to find removing employee");
                    return false;
                }

                db.EmployeeTerritories.RemoveRange(origin.EmployeeTerritories);
                origin.InverseReportsToNavigation.ToList().ForEach(employee => employee.ReportsTo = null);
                db.Employees.Remove(origin);
                origin.Orders.ToList().ForEach(order => order.Employee = null);

                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting employee");
                return false;
            }
        }


    }
}
