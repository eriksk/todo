namespace Todo.Commands;

public interface ICommand<in TOptions>
{
    int Execute(TOptions options);
}
