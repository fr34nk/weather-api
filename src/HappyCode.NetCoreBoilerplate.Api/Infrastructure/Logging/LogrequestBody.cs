
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;



public class LogRequestBodyFilter : ActionFilterAttribute
{
    private readonly ILogger<LogRequestBodyFilter> _logger;

    public LogRequestBodyFilter(ILogger<LogRequestBodyFilter> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var arg in context.ActionArguments)
        {
            _logger.LogInformation("Action Argument: {Key} = {@Value}", arg.Key, arg.Value);
        }
    }
}
