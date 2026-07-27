using System.Net;

namespace lightClinic.Patient.Shared.Interfaces
{
    public interface IResult<T>
    {
        string Message { get; set; }

        bool Succeeded { get; set; }

        T Data { get; set; }

        Dictionary<string, List<string>> Errors { get; set; }

        HttpStatusCode? Code { get; set; }
    }
}