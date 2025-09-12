namespace HotelRoomBooking.Commands;

/// <summary>
/// Command pattern interface for operations that can be executed and potentially undone
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Execute the command
    /// </summary>
    Task ExecuteAsync();

    /// <summary>
    /// Undo the command (if supported)
    /// </summary>
    Task UndoAsync();

    /// <summary>
    /// Check if the command can be undone
    /// </summary>
    bool CanUndo { get; }
}

/// <summary>
/// Result of command execution
/// </summary>
public class CommandResult
{
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }
    public object? Data { get; private set; }
    public Exception? Exception { get; private set; }

    private CommandResult(bool isSuccess, string? message = null, object? data = null, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
        Exception = exception;
    }

    public static CommandResult Success(string? message = null, object? data = null) =>
        new CommandResult(true, message, data);

    public static CommandResult Failure(string message, Exception? exception = null) =>
        new CommandResult(false, message, exception: exception);
}
