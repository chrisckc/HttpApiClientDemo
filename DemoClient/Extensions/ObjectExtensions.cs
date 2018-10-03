using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DemoClient.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// To json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The JSON string representation of any object.</returns>
        public static string ToJson(this object value)
        {
            if (value == null) return null;

            try
            {
                string json = JsonConvert.SerializeObject(value, Formatting.Indented,
                                new JsonConverter[] {new StringEnumConverter()});
                return json;
            }
            catch (Exception exception)
            {
                //log exception but dont throw one
                var error = new { Exception = new { Message = exception.Message} };
                string json = JsonConvert.SerializeObject(error, Formatting.Indented,
                                new JsonConverter[] {new StringEnumConverter()});
                return json;
            }
        }
    }
}