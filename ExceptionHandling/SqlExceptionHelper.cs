using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace TodoListApi.ExceptionHandling
{
    public static class SqlExceptionHelper
    {
        public static bool TyGetExceptionType(DbUpdateException exception, out SqlExceptionType type)
        {
            if(exception is DbUpdateConcurrencyException)
            {
                type = SqlExceptionType.Concurrency;
                return true;
            }

            var ex = exception.InnerException as SqlException;
            if(ex == null)
            {
                type = SqlExceptionType.None;
                return false;
            }

            return TyGetExceptionType(ex, out type);
        }

        public static bool TyGetExceptionType(SqlException exception, out SqlExceptionType type)
        {
            switch (exception.Number)
            {
                case 547:
                    type = SqlExceptionType.Constraint;
                    break;
                case 2627:
                    type = SqlExceptionType.DublicateKey;
                    break;
                default:
                    type = SqlExceptionType.None;
                    return false;
            }

            return true;
        }
    }
}
