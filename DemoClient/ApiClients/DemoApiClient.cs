using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using HttpApiClient;
using HttpApiClient.Models;

namespace DemoClient.ApiClients
{
    public class DemoApiClient : ApiClient<DemoApiClient>
    {
      
        // Demonstrates using sub-classed ApiClientOptions to provide additional options
        public DemoApiClient(HttpClient client, DemoApiClientOptions options, ILogger<DemoApiClient> logger) : base(client, options, logger)
        {
            _logger.LogDebug("DemoApiClient constructed");
            // Log an additional option
            _logger.LogInformation("AnotherOption: {0}", options.AnotherOption);
            
            // We can customise the underlying HttpClient here for example
            if (options.Referrer != null) {
                _client.DefaultRequestHeaders.Referrer = options.Referrer;
            }
            _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true};
        }

        
        // Additional methods which wrap methods in the base class etc.
        public async Task<dynamic> GetData(string resource)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()} : Getting data from resource: {resource}");
            ApiResponse apiResponse = await GetResource(resource);
            bool success = CheckResponse(apiResponse, "demo", "Demo");
            if (success) {
                return apiResponse.Data;
            }
            return null;
        }

        // Do some checking and logging...
        private  bool CheckResponse(ApiResponse apiResponse, string dataRoot, string dataDescription, bool logInfo = true) {
            if (apiResponse == null) return false;
            string methodDesc = "Download";
            if (apiResponse.Method == "PUT") {
                methodDesc = "Updat";
            } else if (methodDesc == "POST") {
                methodDesc = "Creat";
            }
            
            // Log the RetryInfo
            if (apiResponse.RetryInfo != null) {
                _logger.LogWarning($"{DateTime.Now.ToString()} : There were {apiResponse.RetryInfo.RetryCount} request retry attempts due to failures while {methodDesc}ing '{dataDescription}' data\nUrl: {apiResponse.Url}\nRequest Counter: {RequestCounter}");
                StringBuilder sb = new StringBuilder("{DateTime.Now.ToString()} : Request Failures:\n");
                foreach (RetryAttempt retryAttempt in apiResponse.RetryInfo.RetryAttempts) {
                    sb.AppendLine($"Failure {retryAttempt.RetryAttemptNumber} : {retryAttempt.RetryMessage}");
                }
                _logger.LogWarning(sb.ToString());
            }

            // Log some info about the response
            if (apiResponse.Success) {
                dynamic data = null;
                if (string.IsNullOrEmpty(dataRoot)) {
                    data = apiResponse.Data;
                } else {
                    data = apiResponse.Data?.SelectToken(dataRoot);   
                }
                if (data != null && data is JArray) {
                    if (logInfo) _logger.LogInformation($"{DateTime.Now.ToString()} : {methodDesc}ed {data.Count} '{dataDescription}\nUrl: {apiResponse.Url}'");
                } else {
                    if (logInfo) _logger.LogInformation($"{DateTime.Now.ToString()} : {methodDesc}ed '{dataDescription}' data\nUrl: {apiResponse.Url}");
                    //_logger.LogTrace($"'{DateTime.Now.ToString()} : {description}' data:\n{data}");
                }
                _logger.LogTrace($"{dataDescription} Response Data:\n{apiResponse.Data}");
                return true;
            } else {
                _logger.LogError($"{DateTime.Now.ToString()} : Error {methodDesc}ing '{dataDescription}' data\nUrl: {apiResponse.Url}\nStatusCode: {apiResponse.StatusCode} ({apiResponse.StatusText})\nError Title: {apiResponse.ErrorTitle}\nError Type: {apiResponse.ErrorType}\nError Text: {apiResponse.ErrorDetail}\nResponseBody:\n{apiResponse.Body}");
                if (apiResponse.StatusCode.HasValue && apiResponse.StatusCode.Value == 429) {
                    _logger.LogError($"Rate limit was hit while {methodDesc}ing : {apiResponse.Url}");
                } else if (apiResponse.StatusCode.HasValue && apiResponse.StatusCode.Value == 401) {
                    _logger.LogError($"The request was unauthorised");
                } else if (apiResponse.StatusCode.HasValue && apiResponse.StatusCode.Value == 403) {
                    _logger.LogError($"The request was forbidden");
                }
                _logger.LogTrace($"Error: {dataDescription} Response Data:\n{apiResponse.Data}");
                return false;
            }
        }
    }
}
