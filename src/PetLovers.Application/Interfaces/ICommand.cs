using MediatR;

namespace PetLovers.Application.Interfaces;

public interface ICommand : IRequest<CommandResult>
{
}

public interface ICommand<TResponse> : IRequest<CommandResult<TResponse>>
{
}

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, CommandResult>
    where TCommand : ICommand
{
}

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, CommandResult<TResponse>>
    where TCommand : ICommand<TResponse>
{
}

public class CommandResult
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public IReadOnlyList<string> ValidationErrors { get; }

    protected CommandResult(bool isSuccess, string? error, IReadOnlyList<string>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ValidationErrors = validationErrors ?? Array.Empty<string>();
    }

    public static CommandResult Success() => new(true, null);
    public static CommandResult Failure(string error) => new(false, error);
    public static CommandResult ValidationFailure(IReadOnlyList<string> errors) => new(false, "Validation failed", errors);
}

public class CommandResult<T> : CommandResult
{
    public T? Data { get; }

    private CommandResult(bool isSuccess, T? data, string? error, IReadOnlyList<string>? validationErrors = null)
        : base(isSuccess, error, validationErrors)
    {
        Data = data;
    }

    public static CommandResult<T> Success(T data) => new(true, data, null);
    public new static CommandResult<T> Failure(string error) => new(false, default, error);
    public new static CommandResult<T> ValidationFailure(IReadOnlyList<string> errors) => new(false, default, "Validation failed", errors);
}
