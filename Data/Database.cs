using System.Globalization;
using Microsoft.Data.Sqlite;

namespace Todo.Data;

public class Database
{
    private readonly string _connectionString;
    private readonly FileInfo _databaseFile;

    public Database(FileInfo databaseFile)
    {
        if (databaseFile is null)
        {
            throw new ArgumentNullException(nameof(databaseFile));
        }
        _connectionString = CreateConnectionString(databaseFile);
        this._databaseFile = databaseFile;
    }

    private static string CreateConnectionString(FileInfo path) => $"Data source={path.FullName};";

    public void EnsureCreated()
    {
        CreateTables();
    }

    private void CreateTables()
    {
        System.Console.WriteLine(_connectionString);
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
@"CREATE TABLE IF NOT EXISTS Todos(
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Title TEXT, 
Description TEXT, 
Done BOOL, 
Expiration DATE)
";

        command.ExecuteNonQuery();
    }

    public Todo Insert(Todo todo)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
@"INSERT INTO Todos (Title, Description, Done, Expiration)
VALUES (@Title, @Description, @Done, @Expiration)";

        command.Parameters.Add("Title", SqliteType.Text).Value = todo.Title;
        command.Parameters.Add("Description", SqliteType.Text).Value = todo.Description;
        command.Parameters.Add("Done", SqliteType.Integer).Value = false;
        command.Parameters.Add("Expiration", SqliteType.Text).Value = todo.Expiration.ToString(CultureInfo.InvariantCulture);

        command.ExecuteNonQuery();

        return todo;
    }

    public void Complete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
@"UPDATE Todos
SET Done = true
WHERE Id = @Id
";

        command.Parameters.Add("Id", SqliteType.Integer).Value = id;

        _ = command.ExecuteNonQuery();
    }

    public Todo? Get(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
@"SELECT Id, Title, Description, Done, Expiration
FROM Todos
WHERE Id = @Id
";

        command.Parameters.Add("Id", SqliteType.Integer).Value = id;

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Todo()
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Done = reader.GetBoolean(3),
                Expiration = reader.GetDateTime(4)
            };
        }
        return null;
    }

    public IEnumerable<Todo> GetAll()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
    @"SELECT Id, Title, Description, Done, Expiration
FROM Todos
";
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                yield return new Todo()
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Done = reader.GetBoolean(3),
                    Expiration = reader.GetDateTime(4)
                };
            }
        }
    }
}

public class Todo
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool Done { get; set; }
    public DateTime Expiration { get; set; }
}