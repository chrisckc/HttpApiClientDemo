using Microsoft.Extensions.Logging;
using HttpApiClient.ErrorParsers;
using HttpApiClient.Models;

namespace DemoClient.ApiClients
{
    // Example of an error parser for the DemoApiClient
    public class DemoErrorParser : IKnownErrorParser<DemoApiClient>
    {

        private readonly ILogger<DemoErrorParser> _logger;

        public DemoErrorParser(ILogger<DemoErrorParser> logger)
        {
            _logger = logger;
        }

        public bool ParseKnownErrors(ApiResponse apiResponse) {
            bool success = false;
            if (apiResponse != null) {
                _logger.LogDebug($"{this.GetType().ToString()} : Parsing Response Object for Known Errors");

                string test = apiResponse.Data?.Value<string>("test");
                // Try to get an error title
                string errorTitle = apiResponse.Data?.Value<string>("title");
                if (errorTitle == null) errorTitle = apiResponse.Data?.SelectToken("error")?.Value<string>("title");
                if (!string.IsNullOrEmpty(errorTitle)) {
                    apiResponse.ErrorTitle = errorTitle;
                }
                // Try to get an error message
                string errorDetail = apiResponse.Data?.Value<string>("message");
                if (errorDetail == null) errorDetail = apiResponse.Data?.SelectToken("error")?.Value<string>("message");
                if (!string.IsNullOrEmpty(errorDetail)) {
                    apiResponse.ErrorDetail = errorDetail;
                }
                if (!string.IsNullOrEmpty(errorTitle) || !string.IsNullOrEmpty(errorDetail)) {
                    _logger.LogDebug($"{this.GetType().ToString()} : Known Errors have been found!");
                    apiResponse.ErrorType = "ErrorReturnedByServer";
                    success = true;
                }  
            }
            return success;
        }

    }
}