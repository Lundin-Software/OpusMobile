using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using NLog;
using Opus.Mobile.API.Helpers;
using Opus.Mobile.API.Services.Logging;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Shared.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Logger
LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

ConfigurationHelpers.Initialize(builder.Configuration);

#region Swagger Configuration

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Opus Mobile API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

#endregion

builder.Services.AddOpenApi();

#region Services Configuration

//Database
builder.Services.AddDbContext<OpusDBContext>(options =>
    options.UseSqlServer(ConfigurationHelpers.GetMandatoryValue("ConnectionStrings:DefaultConnection"),
    o => o.UseCompatibilityLevel(120)));

//Services
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Opus Mobile API v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#region Redirect IIS blank URL to /swagger

app.MapGet("", (HttpContext httpContext) => Results.Redirect($"{httpContext.Request.PathBase}/swagger"));

#endregion

#region Health Checks

app.MapGet("/health/live", () => Results.Ok(new APIResponse
{
    Success = true,
    Message = "Alive"
}));

app.MapGet("/DBStatus", async (OpusDBContext ctx, ILoggerManager logger) =>
{
    var (success, error) = await HealthChecks.CheckDatabaseAsync(ctx, logger);

    return Results.Ok(new APIResponse
    {
        Success = success,
        Message = success ? "" : error ?? "Unreachable"
    });
})
.WithMetadata(new IgnoreDeviceRestrictionAttribute());

#endregion

app.Run();
