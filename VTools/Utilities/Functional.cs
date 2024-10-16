using System;

namespace VTools.Utilities
{
    public class Error<TError>(TError code, string message) where TError : Enum
    {
        public TError Code { get; } = code;
        public string Message { get; } = message;
    }

    public class Result<T, TError> where TError : Enum
    {
        public static Result<T, TError> From(TError code, string message) => new(new Error<TError>(code, message));
        public static Result<T, TError> From(T value) => new(value);

        public Error<TError>? Error { get; }
        public T? Value { get; }

        private Result(Error<TError> error) => Error = error;
        private Result(T value) => Value = value;
    }
}
