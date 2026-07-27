using FluentValidation.Results;
using lightClinic.Patient.Shared.Extensions;
using lightClinic.Patient.Shared;
using System.Net;

namespace lightClinic.Patient.Shared;

public class PaginatedResult<T> : Result<T>
{
    public PaginatedResult(List<T> data)
    {
        Data = data;
    }

    public PaginatedResult(bool succeeded, List<T> data = default,
                           string message = null,
                           List<ValidationFailure> validationFailures = null,
                           int count = 0,
                           HttpStatusCode httpStatusCode = HttpStatusCode.OK,
                           int pageNumber = 1,
                           int pageSize = 10)
    {
        Data = data;
        CurrentPage = pageNumber;
        Code = httpStatusCode;
        Succeeded = succeeded;
        Message = message;
        Errors = validationFailures?.GetDictionary();
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
    }

    public new List<T> Data { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public static PaginatedResult<T> Create(List<T> data, int count, int pageNumber, int pageSize)
    {
        return new PaginatedResult<T>(true, data, null, null, count, HttpStatusCode.OK, pageNumber, pageSize);
    }
}
