using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Events.Application.DTOs;
using Events.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Events.WebApi.Middleware
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
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, errorDto) = exception switch
            {
                EntityNotFoundException notFoundEx
                    => (HttpStatusCode.NotFound,
                        new ErrorDto 
                        { 
                            Code = "NotFound",
                            Message = notFoundEx.Message
                        }),

                ForbiddenException forbiddenEx
                    => (HttpStatusCode.Forbidden,
                        new ErrorDto 
                        {
                            Code = "Forbidden",
                            Message = forbiddenEx.Message 
                        }),

                ValidationException valEx
                    => (HttpStatusCode.UnprocessableEntity,
                        new ErrorDto
                        {
                            Code = "ValidationError",
                            Message = string.Join("; ", valEx.Errors.Select(e => e.ErrorMessage))
                        }),

                SecurityTokenExpiredException expiredEx
                    => (HttpStatusCode.Unauthorized,
                        new ErrorDto 
                        { 
                            Code = "TokenExpired",
                            Message = expiredEx.Message
                        }),

                SecurityTokenException tokenEx
                    => (HttpStatusCode.Unauthorized,
                        new ErrorDto
                        { 
                            Code = "InvalidToken",
                            Message = tokenEx.Message
                        }),

                UnauthorizedAccessException uaEx
                    => (HttpStatusCode.Unauthorized,
                        new ErrorDto
                        {
                            Code = "Unauthorized", 
                            Message = uaEx.Message 
                        }),

                    _ => (HttpStatusCode.InternalServerError,
                        new ErrorDto
                        {
                            Code = "ServerError",
                            Message = "An unexpected error occurred."
                        })
            };

            context.Response.StatusCode = (int)statusCode;
            var payload = JsonSerializer.Serialize(errorDto);
            return context.Response.WriteAsync(payload);
        }
    }
}
