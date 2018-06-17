using System.Net;

namespace TodoListApi.ExceptionHandling
{
    public class ExceptionHandledResult
    {
        public HttpStatusCode Status { get; }
        public string ErrorMessage { get; }

        public ExceptionHandledResult(HttpStatusCode status, string errorMessage)
        {
            Status = status;
            ErrorMessage = errorMessage;
        }
    }
}
