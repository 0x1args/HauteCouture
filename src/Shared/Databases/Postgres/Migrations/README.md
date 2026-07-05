## Migrations

To simplify working with migrations, it's recommended to move them to a separate ConsoleApp project, which will apply migrations during startup and make it easy to work with migrations in different environments. To do this, add the `Shared.Databases.Postgres.Migrations` package and configure the project for convenience as a host by adding the `Microsoft.Extensions.Hosting package`. Since this project will contain migrations, add the Microsoft.`EntityFrameworkCore.Design` package.

To create migrations, enter the following command, specifying the project where the migrations will be stored and the project that serves as the application's starting point:

```bash
dotnet ef migrations add InitialCreate --context AppDbContext --project src\Modules\<ModuleName>\Hosts\Migrations\<YourSolution>.Modules.<ModuleName>.Hosts.Migrations.csproj --startup-project src\Modules\<ModuleName>\Hosts\Migrations\<YourSolution>.Modules.<ModuleName>.Hosts.Migrations.csproj --output-dir Migrations
```

### Registration 

You need to create migrations for this project E and use the `IMigrationExecutor` interface with the `MigrateAsync` method. To add this to DI, use the following method:

```csharp
var assembly = Assembly.GetExecutingAssembly();
 
services
    .AddDbContext<AppDbContext>(options =>
    {
        // Configuring the connection to Postgres.
        options.UseNpgsql(
            GetConnectionString(configuration),
            optionsBuilder => optionsBuilder.MigrationsAssembly(assembly));
    })
    .AddMigrationExecutor<AppDbContext>();
```