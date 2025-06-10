using Events.Application.Extensions;
using Events.Infrastructure.Extensions;
using Events.Infrastructure.Persistence;
using Events.WebApi;
using Events.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var identityDb = services.GetRequiredService<IdentityDbContext>();
    identityDb.Database.Migrate();

    AdminSeeder.SeedAsync(services).GetAwaiter().GetResult();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Events API V1"));

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
