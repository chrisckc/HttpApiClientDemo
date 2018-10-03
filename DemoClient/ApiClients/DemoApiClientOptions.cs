using System;
using HttpApiClient;

namespace DemoClient.ApiClients
{

    // Custom sub-classed ApiClientOptions class of Type DemoApiClient, inherits all properties from ApiClientOptions
    public class DemoApiClientOptions : ApiClientOptions<DemoApiClient> {
        
        // Some extra options that we would like to use
        public Uri Referrer { get; set; }
        public string AnotherOption { get; set; }

        
    }
}
