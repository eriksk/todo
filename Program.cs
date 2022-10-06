using System;
using CommandLine;
using Todo.Data;

namespace Todo;

public static class Program
{
    [Verb("list", isDefault: false, new[] { "ls" })]
    public class ListOptions
    {
    }

    [Verb("add", isDefault: false)]
    public class AddOptions
    {
        [Option('t', "title", Required = false, HelpText = "Title")]
        public string Title { get; set; }
        [Option('d', "description", Required = false, HelpText = "Description")]
        public string Description { get; set; }
        [Option('e', "expiry", Required = false, HelpText = "Expiration, ex: 1 day, 2 days, 1 hour, tomorrow, today, next week")]
        public string Expiry { get; set; }
    }

    [Verb("done", isDefault: false)]
    public class DoneOptions
    {
        [Option('i', "id", Required = false, HelpText = "Id of the todo")]
        public int Id { get; set; }
    }

    [Verb("database", isDefault: false)]
    public class DatabaseOptions
    {
        [Option('p', "path", Required = false, HelpText = "Set database path")]
        public string Path { get; set; }
    }

    static int Main(params string[] args)
    {
        var databaseFilePath = "todo.db";
        var database = new Database(new FileInfo(databaseFilePath));
        database.EnsureCreated();

        return Parser.Default.ParseArguments<ListOptions, AddOptions, DoneOptions>(args).MapResult(
                (ListOptions listOptions) =>
               {
                   Console.WriteLine("Todos:");

                   foreach (var todo in database.GetAll())
                   {
                       Console.WriteLine(todo.Id + " " + (todo.Done ? "[x]" : "[ ]") + " " + todo.Title + ", " + todo.Description + ", " + todo.Expiration.ToShortDateString());
                   }

                   return 0;
               },
               (AddOptions addOptions) =>
               {
                   database.Insert(new Data.Todo()
                   {
                       Title = addOptions.Title,
                       Description = addOptions.Description,
                       Expiration = DateTime.UtcNow.AddDays(1),
                       Done = false,
                   });

                   Console.WriteLine($"'{addOptions.Title}' added");
                   return 0;
               },
               (DoneOptions doneOptions) =>
               {
                   var todo = database.Get(doneOptions.Id);
                   if (todo == null)
                   {
                       System.Console.WriteLine("Not found 😔");
                       return 1;
                   }

                   if(todo.Done)
                   {
                       System.Console.WriteLine("Already done 😎");
                       return 0;
                   }

                   database.Complete(todo.Id);
                   Console.WriteLine($"'{todo.Title}' completed 👍");
                   return 0;
               },
               errs => 1);
    }
}