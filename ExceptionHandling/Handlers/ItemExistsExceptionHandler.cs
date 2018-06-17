using System;
using TodoListApi.Exceptions;

namespace TodoListApi.ExceptionHandling.Handlers
{
    public class ItemExistsExceptionHandler : IExceptionHandler
    {
        public bool Handle(Exception exception, out ExceptionHandledResult result)
        {
            if (exception is ItemExistsException ex)
            {
                var errorMessage = ex.Id.HasValue ? $"Item '{ex.Id}' is already exists." : "Item is already exists.";
                result = new ExceptionHandledResult(System.Net.HttpStatusCode.Conflict, errorMessage);
                return true;
            }

            result = null;
            return false;
        }
    }
}
