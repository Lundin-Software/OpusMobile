using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NLog;
using Opus.Mobile.API.Helpers;
using Opus.Mobile.API.Middleware;
using Opus.Mobile.API.Services.Articles;
using Opus.Mobile.API.Services.Authentication;
using Opus.Mobile.API.Services.Components;
using Opus.Mobile.API.Services.Logging;
using Opus.Mobile.API.Services.Lookup;
using Opus.Mobile.API.Services.Notifications;
using Opus.Mobile.API.Services.Orders;
using Opus.Mobile.API.Services.Permissions;
using Opus.Mobile.API.Services.Purchases;
using Opus.Mobile.API.Services.TaskLogs;
using Opus.Mobile.API.Services.Tasks;
using Opus.Mobile.API.Services.Todos;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Shared.API;
using System.Text;

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
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
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
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<IComponentService, ComponentService>();
builder.Services.AddScoped<ITaskLogService, TaskLogService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

#endregion

#region Authentication/Authorization

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = ConfigurationHelpers.GetMandatoryValue("Jwt:Issuer"),
        ValidAudience = ConfigurationHelpers.GetMandatoryValue("Jwt:Issuer"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelpers.GetMandatoryValue("Jwt:Secret")))
    };
});

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

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
