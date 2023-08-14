using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public EmployeesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    [HttpPost("CreateEmployee")]
    public async Task<IActionResult> CreateEmployee(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlException)
            {
                Console.WriteLine("Inner Exception: " + ex.Message);
            }
            else
            {
                Console.WriteLine("Error while saving: " + ex.Message);

                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine("Inner Exception: " + innerException.Message);
                    innerException = innerException.InnerException;
                }
            }

        }
        return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
    }

    
    [HttpGet("GetAllEmployees")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        return await _dbContext.Employees.ToListAsync();
    }


    [HttpGet("GetEmployee/{id}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        await Task.Delay(1000);

        var employee = await _dbContext.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        return employee;
    }


    [HttpPut("UpdateEmployee/{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return BadRequest();
        }

        _dbContext.Entry(employee).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmployeeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }
    private bool EmployeeExists(int id)
    {
        return _dbContext.Employees.Any(e => e.Id == id);
    }


    [HttpDelete("DeleteEmployee/{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _dbContext.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet("Search")]
    public async Task<ActionResult<IEnumerable<Employee>>> SearchEmployees([FromQuery] string searchTerm)
    {
        var searchResults = await _dbContext.Employees
            .Where(e => EF.Functions.Like(e.FirstName, $"%{searchTerm}%") || EF.Functions.Like(e.LastName, $"%{searchTerm}%"))
            .ToListAsync();

        return Ok(searchResults);
    }


}

