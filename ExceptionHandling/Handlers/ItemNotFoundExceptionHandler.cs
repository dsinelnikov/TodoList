using System;
using TodoListApi.Exceptions;

namespace TodoListApi.ExceptionHandling.Handlers
{
    public class ItemNotFoundExceptionHandler : IExceptionHandler
    {
        public bool Handle(Exception exception, out ExceptionHandledResult result)
        {
            if(exception is ItemNotFoundException ex)
            {
                result = new ExceptionHandledResult(System.Net.HttpStatusCode.NotFound, $"Item '{ex.Id}' is not found.");
                return true;
            }

            result = null;
            return false;
        }
    }
}
