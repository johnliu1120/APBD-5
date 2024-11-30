using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class AnimalsController(IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    [HttpGet]
    public async Task<IActionResult> GetAnimals(string orderBy = "name")
    {
        var validColumns = new HashSet<string> { "name", "description", "category", "area" };
        if (!validColumns.Contains(orderBy.ToLower()))
            return BadRequest("Invalid sorting column.");

        var query = $"SELECT * FROM Animals ORDER BY {orderBy}";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        using (var command = new SqlCommand(query, connection))
        {
            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();
            var animals = new List<object>();
            while (await reader.ReadAsync())
            {
                animals.Add(new
                {
                    IdAnimal = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Area = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
            return Ok(animals);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddAnimal([FromBody] Animal animal)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var query = "INSERT INTO Animals (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Name", animal.Name);
            command.Parameters.AddWithValue("@Description", animal.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Category", animal.Category ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Area", animal.Area ?? (object)DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return Created("", animal);
        }
    }


    [HttpPut("{idAnimal}")]
    public async Task<IActionResult> UpdateAnimal(int idAnimal, [FromBody] Animal animal)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var query = "UPDATE Animals SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@IdAnimal", idAnimal);
            command.Parameters.AddWithValue("@Name", animal.Name);
            command.Parameters.AddWithValue("@Description", animal.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Category", animal.Category ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Area", animal.Area ?? (object)DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return NotFound();
            return NoContent();
        }
    }


    [HttpDelete("{idAnimal}")]
    public async Task<IActionResult> DeleteAnimal(int idAnimal)
    {
        var query = "DELETE FROM Animals WHERE IdAnimal = @IdAnimal";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@IdAnimal", idAnimal);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return NotFound();
            return NoContent();
        }
    }
}