using FluentValidation.Results;
using lightClinic.Patient.Shared.Extensions;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace lightClinic.Patient.Shared
{
    public class Result<T> : IResult<T>
    {
        public string Message { get; set; }

        public bool Succeeded { get; set; }

        public HttpStatusCode? Code { get; set; }
        public Result(HttpStatusCode statusCode = HttpStatusCode.OK) => Code = statusCode;

        public T Data { get; set; }

        public Dictionary<string, List<string>> Errors { get; set; }

        #region Non Async Methods

        #region Success Methods

        public static Result<T> Success()
        {
            return new Result<T>
            {
                Succeeded = true
            };
        }

        public static Result<T> Success(string message)
        {
            return new Result<T>
            {
                Succeeded = true,
                Message = message
            };
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>
            {
                Succeeded = true,
                Data = data
            };
        }

        public static Result<T> Success(T data, string message)
        {
            return new Result<T>
            {
                Succeeded = true,
                Message = message,
                Data = data
            };
        }

        #endregion Success Methods

        #region Failure Methods

        public static Result<T> Failure(IEnumerable<string> enumerable, HttpStatusCode httpStatusCode)
        {
            return new Result<T>
            {
                Succeeded = false,
                Code = httpStatusCode
            };
        }

        public static Result<T> Failure(string message, HttpStatusCode httpStatusCode)
        {
            return new Result<T>
            {
                Succeeded = false,
                Code = httpStatusCode,
                Message = message
            };
        }

        public static Result<T> Failure(T data, HttpStatusCode httpStatusCode)
        {
            return new Result<T>
            {
                Succeeded = false,
                Code = httpStatusCode,
                Data = data
            };
        }

        public static Result<T> Failure(T data, string message, HttpStatusCode httpStatusCode)
        {
            return new Result<T>
            {
                Succeeded = false,
                Message = message,
                Code = httpStatusCode,
                Data = data
            };
        }

        public static Result<T> ValidationFailure(List<IdentityError> identityErrors, HttpStatusCode httpStatusCode)
        {
            Dictionary<string, List<string>> errors = identityErrors.ToDictionary(x => x.Code, x => new List<string>() { x.Description });

            return new Result<T>
            {
                Succeeded = false,
                Code = httpStatusCode,
                Errors = errors
            };
        }

        public static Result<T> ValidationFailure(List<ValidationFailure> validationFailures, HttpStatusCode httpStatusCode)
        {
            Dictionary<string, List<string>> errors = validationFailures.GetDictionary();

            return new Result<T>
            {
                Succeeded = false,
                Code = httpStatusCode,
                Errors = errors
            };
        }

        #endregion Failure Methods

        #endregion Non Async Methods

        #region Async Methods

        #region Success Methods

        public static Task<Result<T>> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public static Task<Result<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public static Task<Result<T>> SuccessAsync(T data)
        {
            return Task.FromResult(Success(data));
        }

        public static Task<Result<T>> SuccessAsync(T data, string message)
        {
            return Task.FromResult(Success(data, message));
        }

        #endregion Success Methods

        #region Failure Methods

        public static Task<Result<T>> FailureAsync(string message, HttpStatusCode httpStatusCode)
        {
            return Task.FromResult(Failure(message, httpStatusCode));
        }

        public static Task<Result<T>> FailureAsync(List<string> messages, HttpStatusCode httpStatusCode)
        {
            return Task.FromResult(Failure(string.Join(",", messages), httpStatusCode));
        }

        public static Task<Result<T>> FailureAsync(T data, HttpStatusCode httpStatusCode)
        {
            return Task.FromResult(Failure(data, httpStatusCode));
        }

        public static Task<Result<T>> FailureAsync(T data, string message, HttpStatusCode httpStatusCode)
        {
            return Task.FromResult(Failure(data, message, httpStatusCode));
        }

        public static Task<Result<T>> ValidationFailureAsync(List<IdentityError> validationFailures, HttpStatusCode httpStatusCode)
        {
            return Task.FromResult(ValidationFailure(validationFailures, httpStatusCode));
        }

        public static Task<Result<T>> ValidationFailureAsync(List<ValidationFailure> validationFailures, HttpStatusCode httpStatusCode)
        {
            return Task.FromResult(ValidationFailure(validationFailures, httpStatusCode));
        }

        #endregion Failure Methods

        #endregion Async Methods
    }
}