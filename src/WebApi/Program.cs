using Events.Application.Extensions;
using Events.Application.Interfaces;
using Events.Infrastructure.Extensions;
using Events.Infrastructure.Services.Images;
using Events.WebApi;
using Events.WebApi.Middleware;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RegisteredUser", policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Events API", Version = "v1" }));

builder.Services.AddCors(opts =>
  opts.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:3000") 
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()                 
  ));


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Events API V1"));
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
