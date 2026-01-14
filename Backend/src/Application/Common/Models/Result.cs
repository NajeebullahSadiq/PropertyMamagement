namespace WebAPIBackend.Application.Common.Models
{
    /// <summary>
    /// Generic result wrapper for service operations
    /// </summary>
    public class Result
    {
        public bool Succeeded { get; protected set; }
        public string? Error { get; protected set; }
        public string? Message { get; protected set; }

        protected Result(bool succeeded, string? error = null, string? message = null)
        {
            Succeeded = succeeded;
            Error = error;
            Message = message;
        }

        public static Result Success(string? message = null) => new(true, message: message);
        public static Result Failure(string error) => new(false, error);
    }

    /// <summary>
    /// Generic result wrapper with data
    /// </summary>
    public class Result<T> : Result
    {
        public T? Data { get; private set; }

        private Result(bool succeeded, T? data = default, string? error = null, string? message = null)
            : base(succeeded, error, message)
        {
            Data = data;
        }

        public static Result<T> Success(T data, string? message = null) => new(true, data, message: message);
        public static new Result<T> Failure(string error) => new(false, error: error);
    }

    /// <summary>
    /// Validation result for input validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }

        public static ValidationResult Valid() => new() { IsValid = true };
        public static ValidationResult Invalid(string error) => new() { IsValid = false, ErrorMessage = error };
    }
}
