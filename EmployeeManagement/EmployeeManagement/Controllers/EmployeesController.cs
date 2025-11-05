using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeesController(EmployeeDbContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee emp)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var duplicate = await _context.Employees
                .AnyAsync(e => e.Email == emp.Email || e.PhoneNumber == emp.PhoneNumber);

            if (duplicate)
                return Conflict("An employee with this email or phone number already exists.");

            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployees), new { id = emp.Id }, emp);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee emp)
        {
            if (id != emp.Id)
                return BadRequest("Employee ID mismatch");

            var existingEmp = await _context.Employees.FindAsync(id);
            if (existingEmp == null)
                return NotFound("Employee not found");

            // Update fields including new PhoneNumber
            existingEmp.Name = emp.Name;
            existingEmp.Email = emp.Email;
            existingEmp.IsActive = emp.IsActive;
            existingEmp.DateOfJoining = emp.DateOfJoining;
            existingEmp.Salary = emp.Salary;
            existingEmp.PhoneNumber = emp.PhoneNumber;

            await _context.SaveChangesAsync();
            return Ok(existingEmp);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
                return NotFound("Employee not found");

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return Ok("Employee deleted successfully");
        }
    }
}
