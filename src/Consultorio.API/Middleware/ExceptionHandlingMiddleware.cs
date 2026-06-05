using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Consultorio.Application.Exceptions;
using System;
using System.Threading.Tasks;

namespace Consultorio.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro nao tratado");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                NotFoundException => new { statusCode = StatusCodes.Status404NotFound, error = exception.Message },
                ValidationException => new { statusCode = StatusCodes.Status400BadRequest, error = exception.Message },
                UnauthorizedException => new { statusCode = StatusCodes.Status401Unauthorized, error = exception.Message },
                ConflictException => new { statusCode = StatusCodes.Status409Conflict, error = exception.Message },
                _ => new { statusCode = StatusCodes.Status500InternalServerError, error = "Erro interno do servidor" }
            };

            context.Response.StatusCode = (int)(dynamic)response.statusCode;
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
