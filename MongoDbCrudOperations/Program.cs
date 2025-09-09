using MongoDbCrudOperations.Application;
using MongoDbCrudOperations.Configuration;
using MongoDbCrudOperations.Mapper;
using MongoDbCrudOperations.Repository;
using MongoDbCrudOperations.Services;
using MongoDbCrudOperations.Ui;
using MongoDbCrudOperations.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FluentValidation;

var host = CreateHostBuilder(args).Build();

try
{
    var app = host.Services.GetRequiredService<IApplicationService>();
    await app.RunAsync();
}
catch (Exception ex)
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Application terminated unexpectedly");
}
finally
{
    host.Dispose();
}

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                  .AddEnvironmentVariables();
        })
        .ConfigureServices((context, services) =>
        {
            // Configuration
            services.Configure<MongoDbSettings>(
                context.Configuration.GetSection("MongoDb"));

            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Validation
            services.AddScoped<IValidationService, ValidationService>();
            services.AddValidatorsFromAssembly(typeof(BookValidator).Assembly);

            // Repository
            services.AddScoped<IBookRepository, BookRepository>();

            // Services
            services.AddScoped<IBookService, BookService>();

            // Mapper
            services.AddScoped<IMapper, Mapper>();

            // UI
            services.AddScoped<IUiService, UiService>();

            // Application
            services.AddScoped<IApplicationService, ApplicationService>();
        });