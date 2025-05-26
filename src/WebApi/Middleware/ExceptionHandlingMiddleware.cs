using Events.Application.DTOs;
using Events.Domain.Exceptions;
using Events.Infrastructure.Identity;
using Events.WebApi.DTOs.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

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
                        new ErrorDto { Code = "NotFound", Message = notFoundEx.Message }),

                ValidationException valEx
                    => (HttpStatusCode.UnprocessableEntity,
                        new ErrorDto
                        {
                            Code = "ValidationError",
                            Message = string.Join("; ", valEx.Errors)
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
