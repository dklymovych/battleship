using System.Net;

namespace Server.Dto;

public class ResponseDto<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public T? Data { get; set; }
}
