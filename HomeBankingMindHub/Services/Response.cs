using System.Net;

namespace HomeBankingMindHub.Services
{
    public class Response
    {
        public int StatusCode { get; set; }
        public object? Data { get; set; }

        public Response(HttpStatusCode statusCode, object Data)
        {
            StatusCode = (int) statusCode;
            this.Data = Data;
        }

    }
}
