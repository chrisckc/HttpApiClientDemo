using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HttpApiClient;
using HttpApiClient.Models;

namespace DemoClient.ApiClients
{

    public class SampleApiClient : ApiClient<SampleApiClient>
    {
      
        // Demonstrates using the provided ApiClientOptions
        public SampleApiClient(HttpClient client, ApiClientOptions<SampleApiClient> options, ILogger<SampleApiClient> logger) : base(client, options, logger)
        {
            _logger.LogDebug("SampleApiClient constructed");
            // We can customise the HttpClient here
            _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true};
        }

        // Additional methods which wrap methods in the base class etc.
        public async Task<dynamic> GetData(string resource)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()} : Getting data from resource: {resource}");
            ApiResponse apiResponse = await GetResource(resource);
            return apiResponse.Data;
        }
    }
}
