namespace WebApi.Controllers;
using Domain.Exceptions;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Validation;
using ValidationException = FluentValidation.ValidationException;

public static class ControllerExtensions
{
    public static IActionResult ToOk<TResult, TContract>(this Result<TResult> result,
        Func<TResult, TContract> mapper)
    {
        return result.Match(obj =>
        {
            var response = mapper(obj);

            return new OkObjectResult(response);
        }, GenerateActionResult);
    }

    public static IActionResult ToOk<TResult>(this Result<TResult> result)
    {
        return result.Match(response => new OkObjectResult(response), GenerateActionResult);
    }

    private static IActionResult GenerateActionResult(this Exception exception)
    {
        switch (exception)
        {
            case ValidationException validationException:
                return new BadRequestObjectResult(validationException.ToProblemDetails());

            case Domain.Exceptions.ValidationException validationException:
                return new BadRequestObjectResult(validationException.ToProblemDetails());

            case InfraestructureException infrastructureException:
            {
                var problemDetails = new ProblemDetails();
                var statusCode = StatusCodes.Status500InternalServerError;

                problemDetails.Detail = exception.Message;
                problemDetails.Title = infrastructureException.Message;

                return new ObjectResult(problemDetails) { StatusCode = statusCode };
            }

            case UnAuthorizedException unauthorizedException:
                return new ObjectResult(unauthorizedException.ToProblemDetails());

            default:
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
