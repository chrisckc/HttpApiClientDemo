using System;
using System.IO;
using System.Threading;
using HttpApiClient;
using HttpApiClient.ErrorParsers;
using HttpApiClient.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DemoClient.ApiClients;
using DemoClient.Services;
using AutoMapper;

namespace DemoClient
{
    class Program {

        private static IConfiguration Configuration { get; set; }

        private static ILogger<Program> _logger;

        static void Main(string[] args) {
            Console.WriteLine("Starting application...");

            // Build the app configuration
            Console.WriteLine("Building Configuration..");
            IConfiguration configuration = BuildConfiguration();

            // Setup DI
            Console.WriteLine("Configuring Services...");
            var serviceCollection = new ServiceCollection();
            // Call our ConfigureServices method
            ConfigureServices(serviceCollection, configuration);

            // Build the Service Provider so we can use our services
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Get an instance of the logger
            _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            _logger.LogCritical("LogCritical test");
            _logger.LogError("LogError test");
            _logger.LogWarning("LogWarning test");
            _logger.LogInformation("LogInformation test");
            _logger.LogDebug("LogDebug test");
            _logger.LogTrace("LogTrace test");

            // Get our Demo Service
            // GetService<T>() returns null if it can't find the service.
            // GetRequiredService<T>() throws an InvalidOperationException instead.
            var demoService = serviceProvider.GetRequiredService<IDemoService>();

            _logger.LogInformation("Doing Work...");
            CancellationTokenSource cts = new CancellationTokenSource();
            //CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); // Testing cancellation
            
            // This code waits for the result and then logs it
            //bool result = demoService.DoWork(cts).GetAwaiter().GetResult();
            //_logger.LogInformation("Finished, Result: {0}", result);
            
            // This code allows us to cancel by pressing enter
            demoService.DoWork(cts);
            _logger.LogInformation("Press Enter to Cancel");
            
            Console.ReadLine();
            _logger.LogInformation("Cancelling...");
            cts.Cancel();
            _logger.LogInformation("Cancelled");
            Console.ReadLine();
        }

        private static IConfiguration BuildConfiguration() {
            Console.WriteLine("Checking Environment Variables:");
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string verbose = Environment.GetEnvironmentVariable("ASPNETCORE_VERBOSE");
            Console.WriteLine("ASPNETCORE_ENVIRONMENT: {0}", environment);
            Console.WriteLine("ASPNETCORE_VERBOSE: {0}", verbose);
            if (string.IsNullOrWhiteSpace(environment))
            {
                // Default to Production the same as WebHostBuilder does
                Console.WriteLine("Environment not set, using 'Production'");
                environment = "Production";
            }
            if (string.IsNullOrWhiteSpace(verbose)) verbose = "false";

            // Add configuration sources
            // Configuration Builder looks for appsettings.json in the output dir
            // make sure a CopyToOutputDirectory directive is added to csproj file
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration) {
            // Add the configuration to the service collection
            Configuration = (IConfiguration)configuration;
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddOptions();
            services.AddLogging(configure => configure
                .AddConfiguration(configuration.GetSection("Logging"))
                // Add console logger so we can see all the logging produced by the client by default.
                .AddConsole(c => c.IncludeScopes = true)
            );
            // Set logging levels                      
            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
            if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development") {
                // Check for env var named ASPNETCORE_VERBOSE
                if (configuration["ASPNETCORE_VERBOSE"] == "true") {
                    services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Trace);
                } else {
                    services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
                }
            }

            // Add the service that will be making the requests
            services.AddSingleton<IDemoService, DemoService>();

            // Register the sub-classed options for the DemoApiClient populated from the app config
            services.Configure<DemoApiClientOptions>(configuration.GetSection("DemoApi"));
            // Add our custom KnownErrorParser of the correct type
            services.AddSingleton<IKnownErrorParser<DemoApiClient>, DemoErrorParser>();

            // Configures the ApiClient and provides the sub-classed options type
            // Options specified in the configuration lambda override the ones from configuration
            services.AddApiClient<DemoApiClient, DemoApiClientOptions>(options => {
                //options.BaseUrl = new Uri(configuration["Api:BaseUrl"]);
                //options.UserAgent = "DemoApiClient";
                //options.RequestTimeout = 60;
                //options.RetryCount = 3;
                //options.RetryWaitDuration = 2;
                //options.DefaultTooManyRequestsRetryDuration = 60;
                options.UseExponentialRetryWaitDuration = true;
                options.Referrer = new Uri("https://localhost");
            });

            // Register the options of the correct type for the SampleApiClient populated from the app config
            services.Configure<ApiClientOptions<SampleApiClient>>(configuration.GetSection("SampleApi"));

            // Configures the SampleApiClient using ApiClientOptions of same type as the client
            services.AddApiClient<SampleApiClient>(options => {
                //options.BaseUrl = new Uri(configuration["Api:BaseUrl"]);
                //options.UserAgent = "SampleApiClient";
                //options.RequestTimeout = 60;
                //options.RetryCount = 3;
                //options.RetryWaitDuration = 2;
                //options.DefaultTooManyRequestsRetryDuration = 60;
                options.UseExponentialRetryWaitDuration = false;
            });

            services.AddAutoMapper();
        }
    }
}