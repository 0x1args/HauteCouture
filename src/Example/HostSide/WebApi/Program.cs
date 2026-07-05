using HauteCouture.Example.HostSide.Public.WebApi.Registration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;
var host = builder.Host;

services.AddExampleWebApi(configuration, environment, host);

var app = builder.Build();

app.UseExampleWebApi();

await app.RunAsync();