namespace Todo.Commands;

public static class ListCommand
{
    [Verb("list", isDefault: false, new[] { "ls" })]
    public class Options
    {
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
            Console.WriteLine("Todos:");

            foreach (var todo in _database.GetAll())
            {
                Console.WriteLine(todo.Id + " " + (todo.Done ? "[x]" : "[ ]") + " " + todo.Title + ", " + todo.Description + ", " + todo.Expiration.ToShortDateString());
            }

            return 0;
        }
    }
}