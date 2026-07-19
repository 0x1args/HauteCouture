using HauteCouture.TenantManagement.HostSide.WebApi.Registration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;
var host = builder.Host;

services.AddTenantManagementWebApi(
    configuration,
    environment,
    host);

var app = builder.Build();
app.UseTenantManagementWebApi();

await app.RunAsync();