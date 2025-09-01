using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HappyCode.NetCoreBoilerplate.Api.Infrastructure.Filters
{
    [ExcludeFromCodeCoverage]
    public class HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger) : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {

            Console.Write("An API internal error has occurred");
            logger.LogError(context.Exception, "An API internal error has occurred");

            var ex = context.Exception;

            string message = ex?.Message ?? "An unexpected error occurred.";

            if (ex?.InnerException != null)
            {
                message += " | Inner: " + ex.InnerException.Message;
            }

            string dataErrors = "";
            if (ex?.Data != null && ex.Data.Count > 0)
            {
                foreach (var key in ex.Data.Keys)
                {
                    dataErrors += $"{key}: {ex.Data[key]} ";
                }
            }

            var problem = new ProblemDetails
            {
                Title = "Unhandled exception",
                Detail = message + (string.IsNullOrWhiteSpace(dataErrors) ? "" : " | Data: " + dataErrors),
                Status = 500
            };

            context.Result = new ObjectResult(problem)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;

            if (env.IsDevelopment())
            {
                throw context.Exception;
            }

            logger.LogError(context.Exception, "An API internal error has occurred");

            var errorResponse = new HttpValidationProblemDetails { Title = "An internal error has occurred" };
            context.Result = new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status500InternalServerError };

            context.ExceptionHandled = true;
        }
    }
}
