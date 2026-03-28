using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.Exceptions;

namespace TourDocs.API.Filters;

/// <summary>
/// Global exception filter that catches custom exceptions and returns proper ProblemDetails responses.
/// </summary>
public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case NotFoundException notFoundEx:
                _logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
                context.Result = new NotFoundObjectResult(
                    ApiResponse<object>.Failure(notFoundEx.Message));
                context.ExceptionHandled = true;
                break;

            case ForbiddenException forbiddenEx:
                _logger.LogWarning(forbiddenEx, "Forbidden: {Message}", forbiddenEx.Message);
                context.Result = new ObjectResult(
                    ApiResponse<object>.Failure(forbiddenEx.Message))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                context.ExceptionHandled = true;
                break;

            case Core.Exceptions.ValidationException validationEx:
                _logger.LogWarning(validationEx, "Validation failed: {Message}", validationEx.Message);
                context.Result = new BadRequestObjectResult(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = validationEx.Message,
                        Errors = validationEx.Errors
                    });
                context.ExceptionHandled = true;
                break;

            case BusinessRuleException businessEx:
                _logger.LogWarning(businessEx, "Business rule violation: {Message}", businessEx.Message);
                context.Result = new ObjectResult(
                    ApiResponse<object>.Failure(businessEx.Message))
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
                context.ExceptionHandled = true;
                break;

            default:
                _logger.LogError(context.Exception, "Unhandled exception: {Message}", context.Exception.Message);
                context.Result = new ObjectResult(
                    ApiResponse<object>.Failure("An unexpected error occurred."))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                context.ExceptionHandled = true;
                break;
        }
    }
}
