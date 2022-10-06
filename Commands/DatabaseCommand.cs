namespace Todo.Commands;

public static class DatabaseCommand
{
    [Verb("database", isDefault: false)]
    public class Options
    {
        [Option('p', "path", Required = false, HelpText = "Set database path")]
        public string? Path { get; set; }
    }

    public class Handler : ICommand<Options>
    {
        private readonly Database _database;

        public Handler(Database database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public int Execute(Options options)
        {
            throw new NotImplementedException("Todo: database operations");
        }
    }
}