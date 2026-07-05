using HauteCouture.Example.HostSide.Private.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HauteCouture.Example.HostSide.Private.Migrations.Registration;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddMigrationHost(configuration);

var app = builder.Build();

var migrator = app.Services.GetRequiredService<MigrationStartup>();
using var cancellationTokenSource = new CancellationTokenSource();
await migrator.StartAsync(cancellationTokenSource.Token);

return 0;