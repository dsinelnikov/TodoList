using System;

namespace TodoListApi.ExceptionHandling
{
    public interface IExceptionHandler
    {
        bool Handle(Exception exception, out ExceptionHandledResult result);
    }
}
