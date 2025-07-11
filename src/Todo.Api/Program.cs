using Todo.Api;
using Todo.Api.Hubs;
using Todo.Api.Settings;
using Todo.Application.Extensions;
using Todo.Application.Models;
using Todo.Application.Settings;
using Todo.Core.Users;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() 
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicyLocalHost", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();

var loggerFactory = AppOpenTelemetry.CreateLoggerFactory(
    builder.Configuration.GetRequiredSetting<string>(SettingsConstants.ApplicationInsights),
    builder.Configuration.GetRequiredSetting<OpenTelemetrySettings>());

builder.Services.Configure<HubSettings>(options => builder.Configuration.GetSection(SettingsConstants.HubSettings).Bind(options));

builder.Services.AddSingleton(loggerFactory);
builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

builder.Services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
builder.Services.AddSingleton<IUserMessageSender, UserMessageSender>();

builder.Services.AddCoreServices(builder.Configuration, loggerFactory);

var app = builder.Build();

app.Lifetime.ApplicationStopping.Register(AppOpenTelemetry.Dispose);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapHub<UserHub>(builder.Configuration.GetRequiredSetting<string>(SettingsConstants.HubSettingsUrl));

app.UseHttpsRedirection();

app.UseCors("CorsPolicyLocalHost");

app.UseAuthorization();

app.MapControllers();

app.Run();
