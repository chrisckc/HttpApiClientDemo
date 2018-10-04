using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DemoClient.ApiClients;
using AutoMapper;
using DemoClient.Models;
using DemoClient.Extensions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DemoClient.Services
{
    // Demo Service that uses 2 ApiClients
    public class DemoService : IDemoService
    {
        private DemoApiClient _demoApiClient;
        private SampleApiClient _sampleApiClient;

        private readonly IMapper _mapper;
        private readonly ILogger<DemoService> _logger;
        
        public DemoService(DemoApiClient demoApiClient, SampleApiClient sampleApiClient, IMapper mapper, ILogger<DemoService> logger)
        {
            _demoApiClient = demoApiClient;
            _sampleApiClient = sampleApiClient;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug("DemoService constructed");
        }

        public async Task<bool> DoWork(CancellationTokenSource cancellationTokenSource)
        {
            _logger.LogInformation("{0} : DemoService is doing work...", DateTime.Now.ToString());

            // Call the Demo server using the DemoApiClient
            bool demoResult = await CallDemoServer(cancellationTokenSource);
            
            // Call a different server using the SampleApiClient
            //bool sampleResult = await CallSampleServer(cancellationTokenSource);

            // All done
            return demoResult;
        }

        // Test different response status codes etc.
        private async Task<bool> CallDemoServer(CancellationTokenSource cancellationTokenSource)
        {
            _demoApiClient.CancellationTokenSource = cancellationTokenSource;

            // Demonstrate the mapping of JObject and JArray to a POCO or DTO 
            await JObjectMappingDemo();
            await JArrayMappingDemo();

            var apiResponse200 = await _demoApiClient.GetResource("Demo/Get200/1");
            var obj = new { Demo = new { Title = "Get200 1", Message = "Ok 1", Timestamp = DateTime.Now } };
            var apiResponsePost201 = await _demoApiClient.PostResource("Demo/Post201", obj);
            var apiResponsePut200 = await _demoApiClient.PutResource("Demo/Put200", obj);
            var apiResponseDelete202 = await _demoApiClient.DeleteResource("Demo/Delete202/1");
            var apiResponseDelete204 = await _demoApiClient.DeleteResource("Demo/Delete204/1");
            var apiResponse400 = await _demoApiClient.GetResource("Demo/Get400");
            var apiResponse401 = await _demoApiClient.GetResource("Demo/Get401");
            var apiResponse403 = await _demoApiClient.GetResource("Demo/Get403");
            var apiResponse409 = await _demoApiClient.GetResource("Demo/Get409");

            // Status codes 408, 429, 500, 502, 503, 504 result in retries
            var apiResponse408 = await _demoApiClient.GetResource("Demo/Get408");
            // Simulates a TooManyRequests error with a Retry-After header
            var apiResponse429_80 = await _demoApiClient.GetResource("Demo/Get429/80");
            var apiResponse429_35 = await _demoApiClient.GetResource("Demo/Get429/35");
            var apiResponse500 = await _demoApiClient.GetResource("Demo/Get500");

            // Create a delay longer than the default 60 second ApiClient timeout
            var apiResponseDelay70 =  await _demoApiClient.GetResource("Demo/GetDelay/70");

            return true;
        }

        // Demonstrate the mapping of JObject to a POCO or DTO 
        private async Task<bool> JObjectMappingDemo()
        {
            // Returns a JObject from the root object 'demo'
            var data = await _demoApiClient.GetData("Demo/Get200/1", "demo");
            
            // Map using Newtonsoft.Json's JsonSerializer via the PopulateObject extension method
            // This works if the property names match, differences in case don't matter
            // Refer to Extensions/JsonExtensions.cs
            DemoModel demoModel =  new DemoModel();
            data.PopulateObject<DemoModel>(demoModel);
            // Note: For mapping non matching property names we could use a Custom JsonConverter

            // If you like Automapper
            // Map using Automapper (actually uses JsonSerializer in the config)
            // Must use an existing object for this mapping config
            // Refer to MappingProfile.cs for the Mapping config 
            DemoModel demoModel2 = new DemoModel();
            _mapper.Map(data, demoModel2);

            // Use Automapper to Map to an object where the property names don't match
            // Refer to MappingProfile.cs for the Mapping config
            // Because we are using a specific mapping for ExampleModel we can let automapper create the object
            // For the reason why, refer to the MappingProfile.cs
		    ExampleModel exampleModel = _mapper.Map<ExampleModel>(data);

            return true;
        }

        // Demonstrate the mapping of JArray to a List of POCO's or DTO's 
        private async Task<bool> JArrayMappingDemo()
        {
            // Returns a JArray from the root object 'demos'
            var data = await _demoApiClient.GetData("Demo/Get200", "demos");
            
            // Map using Newtonsoft.Json's JsonSerializer via the PopulateObject extension method
            // This works if the property names match, differences in case don't matter
            // Refer to Extensions/JsonExtensions.cs
            List<DemoModel> demoModels = new List<DemoModel>();
            data.PopulateObject<List<DemoModel>>(demoModels);
            // Note: For mapping non matching property names we could use a Custom JsonConverter

            // If you like Automapper
            // Map using Automapper (actually uses JsonSerializer in the config)
            // Must use an existing object for this mapping config
            // Refer to MappingProfile.cs for the Mapping config 
            List<DemoModel> demoModels2 = new List<DemoModel>();
            _mapper.Map(data, demoModels2);

            // Use Automapper to Map to an object where the property names don't match
            // Refer to MappingProfile.cs for the Mapping config 
            // Because we are using a specific mapping for ExampleModel we can let automapper create the object
            // For the reason why, refer to the MappingProfile.cs
            List<ExampleModel> exampleModels = _mapper.Map<List<ExampleModel>>(data);

            return true;
        }

        // Call a different server using a different ApiClient...
        // private async Task<bool> CallSampleServer(CancellationTokenSource cancellationTokenSource)
        // {
        //     _sampleApiClient.CancellationTokenSource = cancellationTokenSource;
        //     var apiResponse200 = await _sampleApiClient.GetResource("Sample/Get200/1");
        //     // etc. .....
        //     return true;
        // } 
        
    }
}
