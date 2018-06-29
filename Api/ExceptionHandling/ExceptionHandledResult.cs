using System.Net;

namespace TodoList.ExceptionHandling
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
