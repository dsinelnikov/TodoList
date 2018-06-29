using System;

namespace TodoList.ExceptionHandling
{
    public interface IExceptionHandler
    {
        bool Handle(Exception exception, out ExceptionHandledResult result);
    }
}
