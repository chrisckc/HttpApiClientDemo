using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemoClient.Extensions
{
    public static class JsonExtensions
    {
        public static void PopulateObject<T>(this JToken jToken, T obj)
        {
            if (jToken == null) {
                throw new System.ArgumentNullException(nameof(jToken), "Cannot Populate an object from a null JToken");
            }
            if (obj == null) {
                throw new System.ArgumentNullException(nameof(obj), "Cannot Populate a null object");
            }
            JsonSerializer serializer = new JsonSerializer();
            serializer.Populate(jToken.CreateReader(), obj);

            // Probably best to thrown an exception instead
            // if (jToken != null) {
            //     JsonSerializer serializer = new JsonSerializer();
            //     serializer.Populate(jToken.CreateReader(), obj);
            // }
        }
    }
}
