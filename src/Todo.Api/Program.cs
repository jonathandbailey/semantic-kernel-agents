using Todo.Api.Hubs;
using Todo.Core.Extensions;
using Todo.Core.Interfaces;
using Todo.Core.Models;
using Todo.Core.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var loggerFactory = AppOpenTelemetry.CreateLoggerFactory(
    builder.Configuration.GetRequiredSetting<string>(SettingsConstants.ApplicationInsights),
    builder.Configuration.GetRequiredSetting<OpenTelemetrySettings>());

builder.Services.AddSingleton(loggerFactory);
builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

builder.Services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
builder.Services.AddSingleton<IUserMessageSender, UserMessageSender>();

builder.Services.AddCoreServices(builder.Configuration, loggerFactory);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<UserHub>("/hub");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
