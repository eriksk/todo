namespace Todo.Commands;

public static class AddCommand
{
    [Verb("add", isDefault: false)]
    public class Options
    {
        [Option('t', "title", Required = false, HelpText = "Title")]
        public string? Title { get; set; }
        [Option('d', "description", Required = false, HelpText = "Description")]
        public string? Description { get; set; }
        [Option('e', "expiry", Required = false, HelpText = "Expiration, ex: 1 day, 2 days, 1 hour, tomorrow, today, next week")]
        public string? Expiry { get; set; }
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
            _database.Insert(new Data.Todo()
            {
                Title = options.Title,
                Description = options.Description,
                Expiration = DateTime.UtcNow.AddDays(1),
                Done = false,
            });

            Console.WriteLine($"'{options.Title}' added");
            return 0;
        }
    }
}
