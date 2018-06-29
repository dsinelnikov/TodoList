using System;
using System.Net;

namespace TodoList.ExceptionHandling.Handlers
{
    public class UnhandledExceptionHandler : IExceptionHandler
    {
        public bool Handle(Exception exception, out ExceptionHandledResult result)
        {
            result = new ExceptionHandledResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            return true;
        }
    }
}
