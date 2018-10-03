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
                // Try to get an error title
                string errorTitle = apiResponse.Data?.SelectToken("title");
                if (errorTitle == null) errorTitle = apiResponse.Data?.SelectToken("error")?.SelectToken("title")?.Value;
                if (!string.IsNullOrEmpty(errorTitle)) {
                    apiResponse.ErrorTitle = errorTitle;
                }
                // Try to get an error message
                string errorDetail = apiResponse.Data?.SelectToken("message");
                if (errorDetail == null) errorDetail = apiResponse.Data?.SelectToken("error")?.SelectToken("message")?.Value;
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