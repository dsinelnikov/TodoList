using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TodoList.Core.Exceptions;

namespace TodoList.ExceptionHandling.Handlers
{
    public class DataAccessExceptionHandlers : IExceptionHandler
    {
        private readonly IDictionary<Type, HttpStatusCode> _exceptionResults;

        public DataAccessExceptionHandlers()
        {
            _exceptionResults = new Dictionary<Type, HttpStatusCode>()
            {
                { typeof(ItemExistsException), HttpStatusCode.Conflict },
                { typeof(ItemNotFoundException), HttpStatusCode.NotFound },
                { typeof(DataCorruptedException), HttpStatusCode.InternalServerError },
                { typeof(TimeoutException), HttpStatusCode.InternalServerError }
            };
        }

        public bool Handle(Exception exception, out ExceptionHandledResult result)
        {
            if(_exceptionResults.TryGetValue(exception.GetType(), out var status))
            {
                result = new ExceptionHandledResult(status, exception.Message);
                return true;
            }

            result = null;
            return false;
        }
    }
}
