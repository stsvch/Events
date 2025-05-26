using Events.Application.Extensions;
using Events.Application.Interfaces;
using Events.Infrastructure.Extensions;
using Events.Infrastructure.Services.Caching;
using Events.Infrastructure.Services.Images;
using Events.WebApi.Middleware;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IImageStorageService>(sp =>
    new CachedImageService(
        sp.GetRequiredService<CloudinaryImageService>(),
        sp.GetRequiredService<IDistributedCache>()
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Events API", Version = "v1" });
});

builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
);

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
