using Hermes.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UrlShortener.Client;
using hermes_tester_api.Models.Configuration;
using hermes_tester_api.Services.Interfaces;
using hermes_tester_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var environmentOptions = new EnvironmentOptions();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMessagingService, MessagingService>();
builder.Services.AddSingleton<IHermesClient, HermesClient>(_ => new HermesClient(new HermesClientConfig
{
    HermesBaseUri = environmentOptions.HermesUri,
    HermesUserName = environmentOptions.ServiceUsername,
    HermesPassword = environmentOptions.ServicePassword
}));
builder.Services.AddUrlShortenerClient
(
    environmentOptions.UrlShortenerUri,
    environmentOptions.ServiceUsername,
    environmentOptions.ServicePassword
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();