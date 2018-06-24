using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;

namespace TodoListApi.ExceptionHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IExceptionHandler> _handlers;

        public ErrorHandlingMiddleware(RequestDelegate next, IEnumerable<IExceptionHandler> handlers)
        {
            _next = next;
            _handlers = handlers.ToArray();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if(!await HandleExceptionAsync(httpContext, ex))
                {
                    throw;
                }
            }
        }

        private async Task<bool> HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ExceptionHandledResult handlerResult = null;
            if(_handlers.Any(h => h.Handle(exception, out handlerResult)))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)handlerResult.Status;

                var result = JsonConvert.SerializeObject(new { Error = handlerResult.ErrorMessage });
                await context.Response.WriteAsync(result);
            }

            return true;
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder, IEnumerable<IExceptionHandler> handlers)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>(handlers);
        }
    }
}
