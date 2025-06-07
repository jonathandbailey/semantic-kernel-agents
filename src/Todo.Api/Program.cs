using Todo.Api.Hubs;
using Todo.Api.Settings;
using Todo.Application.Extensions;
using Todo.Application.Interfaces;
using Todo.Application.Models;
using Todo.Application.Settings;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.MapHub<UserHub>(builder.Configuration.GetRequiredSetting<string>(SettingsConstants.HubSettingsUrl));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
