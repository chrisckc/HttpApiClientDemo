using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DemoClient.ApiClients;

namespace DemoClient.Services
{
    // Demo Service that uses 2 ApiClients
    public class DemoService : IDemoService
    {
        private DemoApiClient _demoApiClient;
        private SampleApiClient _sampleApiClient;
        private readonly ILogger<DemoService> _logger;
        
        public DemoService(DemoApiClient demoApiClient, SampleApiClient sampleApiClient, ILogger<DemoService> logger)
        {
            _demoApiClient = demoApiClient;
            _sampleApiClient = sampleApiClient;
            _logger = logger;
            _logger.LogDebug("DemoService constructed");
        }

        public async Task<bool> DoWork(CancellationTokenSource cancellationTokenSource)
        {
            _logger.LogInformation("{0} : DemoService is doing work...", DateTime.Now.ToString());

            bool demoResult = await CallDemoServer(cancellationTokenSource);
            //bool sampleResult = await CallSampleServer(cancellationTokenSource);

            // All done
            return demoResult;
        }

        // Test different response status codes
        private async Task<bool> CallDemoServer(CancellationTokenSource cancellationTokenSource)
        {
            _demoApiClient.CancellationTokenSource = cancellationTokenSource;
            // await _demoApiClient.GetData("Demo/Get200");
            // await _demoApiClient.GetData("Demo/Get201");
            // await _demoApiClient.GetData("Demo/Get400");
            // await _demoApiClient.GetData("Demo/Get401");
            // await _demoApiClient.GetData("Demo/Get403");
            // await _demoApiClient.GetData("Demo/Get408");
            // await _demoApiClient.GetData("Demo/Get409");
            // // Simulates a TooManyRequests error with a Retry-After header
            // await _demoApiClient.GetData("Demo/Get429/80");
            // await _demoApiClient.GetData("Demo/Get429/35");
            await _demoApiClient.GetData("Demo/Get500");
            // Get a delay longer than the default 60 second ApiClient timeout
            //await _demoApiClient.GetData("Demo/GetDelay/70");

            return true;
        }

        // private async Task<bool> CallSampleServer(CancellationTokenSource cancellationTokenSource)
        // {
        //     _sampleApiClient.CancellationTokenSource = cancellationTokenSource;
        //     await _sampleApiClient.GetData("Sample/Get200");
        //     // etc. .....
        //     return true;
        // } 
        
    }
}
