namespace Todo;

public static class Program
{
    static int Main(params string[] args)
    {
        // TODO: Store path to database in Local AppData
        var databaseFilePath = "todo.db";
        var database = new Database(new FileInfo(databaseFilePath));
        database.EnsureCreated();

        return Parser.Default.ParseArguments<
            ListCommand.Options,
            AddCommand.Options,
            DoneCommand.Options>(args).MapResult(
               (ListCommand.Options options) => new ListCommand.Handler(database).Execute(options),
               (AddCommand.Options options) => new AddCommand.Handler(database).Execute(options),
               (DoneCommand.Options options) => new DoneCommand.Handler(database).Execute(options),
               errs => 1);
    }
}