namespace Todo.Commands;

public static class DoneCommand
{
    [Verb("done", isDefault: false)]
    public class Options
    {
        [Option('i', "id", Required = false, HelpText = "Id of the todo")]
        public int Id { get; set; }
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
            var todo = _database.Get(options.Id);
            if (todo == null)
            {
                System.Console.WriteLine("Not found 😔");
                return 1;
            }

            if (todo.Done)
            {
                System.Console.WriteLine("Already done 😎");
                return 0;
            }

            _database.Complete(todo.Id);
            Console.WriteLine($"'{todo.Title}' completed 👍");
            return 0;
        }
    }
}
