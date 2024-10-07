using InterviewTest.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace InterviewTest.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class EmployeesController : ControllerBase
    {
        [HttpGet]
        public List<Employee> GetEmployeeList()
        {
            var employees = new List<Employee>();

            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var queryCmd = connection.CreateCommand();
                queryCmd.CommandText = @"SELECT Id,Name, Value FROM Employees";
                using (var reader = queryCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Value = reader.GetInt32(2)
                        });
                    }
                }
            }

            return employees;
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                // Check if the employee exists before attempting to delete
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT COUNT(*) FROM Employees WHERE Id = @id";
                checkCmd.Parameters.AddWithValue("@id", id);
                var exists = (long)checkCmd.ExecuteScalar() > 0;

                if (!exists)
                {
                    return NotFound();
                }

                // Delete the employee
                var deleteCmd = connection.CreateCommand();
                deleteCmd.CommandText = "DELETE FROM Employees WHERE Id = @id";
                deleteCmd.Parameters.AddWithValue("@id", id);
                deleteCmd.ExecuteNonQuery();
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employee updatedEmployee)
        {
            // Check if the updatedEmployee is null
            if (updatedEmployee == null)
            {
                return BadRequest("Invalid employee data.");
            }

            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                // Check if the employee exists before attempting to update
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT Name FROM Employees WHERE Id = @id";
                checkCmd.Parameters.AddWithValue("@id", id);
                var currentName = checkCmd.ExecuteScalar() as string;

                if (currentName == null)
                {
                    return NotFound();
                }

               
                if (currentName != updatedEmployee.Name)
                {
                    // Update the employee's details
                    var updateCmd = connection.CreateCommand();
                    updateCmd.CommandText = @"UPDATE Employees SET Name = @name, Value = 
                                              CASE WHEN @name LIKE 'E%' THEN Value + 1 
                                              WHEN @name LIKE 'G%' THEN Value + 10 
                                              ELSE Value + 100 
                                              END WHERE Id = @id";

                    updateCmd.Parameters.AddWithValue("@name", updatedEmployee.Name);
                    updateCmd.Parameters.AddWithValue("@id", id);

                    updateCmd.ExecuteNonQuery();
                    return NoContent();
                }
                else
                {
                    return Ok("No changes made. The name is the same.");
                }
            }
        }

        [HttpPost]
        public IActionResult AddEmployee([FromBody] Employee newEmployee)
        {
            // Validate the incoming employee data
            if (newEmployee == null || string.IsNullOrEmpty(newEmployee.Name) || newEmployee.Value < 0)
            {
                return BadRequest("Invalid employee data.");
            }

            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                // Insert the new employee into the database
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"INSERT INTO Employees (Name, Value)
                                        VALUES (@name, CASE WHEN @name LIKE 'E%' THEN 1 
                                                            WHEN @name LIKE 'G%' THEN 10 
                                                            ELSE 100 END)";
                insertCmd.Parameters.AddWithValue("@name", newEmployee.Name);
               

                insertCmd.ExecuteNonQuery();
            }

            return CreatedAtAction(nameof(GetEmployeeList), new { name = newEmployee.Name }, newEmployee);
        }

       

        [HttpGet]
        public IActionResult GetSumOfValues()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var queryCmd = connection.CreateCommand();
                queryCmd.CommandText = @"SELECT SUM(Value) FROM Employees WHERE Name LIKE 'A%' OR Name LIKE 'B%' OR Name LIKE 'C%'
                                         HAVING SUM(Value) >= 11171";

                using (var reader = queryCmd.ExecuteReader())
                {
                    var sums = new List<int>();
                    while (reader.Read())
                    {
                        sums.Add(reader.GetInt32(0));
                    }

                    return Ok(sums);
                }
            }
        }

    }
}
