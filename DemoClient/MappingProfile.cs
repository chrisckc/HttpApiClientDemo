using System.Collections.Generic;
using AutoMapper;
using DemoClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemoClient
{
    // Creates the Automapper profile
    // Works with services.AddAutoMapper() which searches for this class
    // Works only if this is file in the same assembly
    public class MappingProfile : Profile {
        public MappingProfile() {
            // Used the JsonTypeConverter method instead
            // CreateMap<JObject, object>().ConvertUsing((jobj, obj) =>
            // {
            //     JsonSerializer serializer = new JsonSerializer();
            //     serializer.Populate(jobj.CreateReader(), obj);
            //     return obj;
            // });

            // // Separate mappings are required for JObject and JArray despite the mapping code being the same
            // // This is because the top level JToken type is to generic for a mapping to be specified

            // // Creates a generic mapping from a JObject to any object
            // CreateMap<JObject, object>().ConvertUsing(new JObjectTypeConverter());

            // // Creates a generic mapping from a JArray to any object
            // CreateMap<JArray, object>().ConvertUsing(new JArrayTypeConverter());

            // Edit: we can use JContainer to cover both JArray and JObject without capturing JValue
            // https://stackoverflow.com/questions/38558844/jcontainer-jobject-jtoken-and-linq-confusion
            // Capturing JValue breaks the ExampleModel specific mapping below
            // Create a generic mapping from a JContainer to any object
            CreateMap<JContainer, object>().ConvertUsing(new JContainerTypeConverter());

            // To map ExampleModel which has non-matching property names we need an explicit mapping for the object
            // and also an explicit mapping for the List of objects
            // Use the Generic JArrayToListConverter to reduce code duplication
            CreateMap<JArray, List<ExampleModel>>().ConvertUsing<JArrayToListConverter<ExampleModel>>();
            // For mapping to ExampleModel, create a specific mapping for each property
            CreateMap<JObject, ExampleModel>()
                .ForMember("Heading", 
                        options => options.MapFrom(jo => jo["title"]))
                .ForMember("Text", 
                        options => options.MapFrom(jo => jo["message"]))
                .ForMember("Date", 
                        options => options.MapFrom(jo => jo["timestamp"]));
        }
    }

    // TypeConverter for JContainer that uses JsonSerializer
    // Covers both JObject and JArray and also JProperty, but not JValue
    public class JContainerTypeConverter : ITypeConverter<JContainer, object>
    {
        public object Convert(JContainer source, object destination, ResolutionContext context)
        {
            if (source == null) {
                throw new System.ArgumentNullException(nameof(source), "Cannot Map from a null JContainer");
            }
            if (destination == null) {
                throw new System.ArgumentNullException(nameof(destination), "Cannot Map to a null object");
            }
            JsonSerializer serializer = new JsonSerializer();
            serializer.Populate(source.CreateReader(), destination);
            return destination;
        }
    }

    // TypeConverter for JObject that uses JsonSerializer
    // This is now redundant in favour of the JContainerTypeConverter
    public class JObjectTypeConverter : ITypeConverter<JObject, object>
    {
        public object Convert(JObject source, object destination, ResolutionContext context)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Populate(source.CreateReader(), destination);
            return destination;
        }
    }

    // TypeConverter for JArray that uses JsonSerializer
    // This is now redundant in favour of the JContainerTypeConverter
    public class JArrayTypeConverter : ITypeConverter<JArray, object>
    {
        public object Convert(JArray source, object destination, ResolutionContext context)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Populate(source.CreateReader(), destination);
            return destination;
        }
    }

    // A Generic class to map a JArray to a List<T>
    public class JArrayToListConverter<T> : ITypeConverter<JArray, List<T>> where T : new()
    {
        public List<T> Convert(JArray source, List<T> destination, ResolutionContext context)
        {
            if (source == null) {
                throw new System.ArgumentNullException(nameof(source), "Cannot Map from a null JArray");
            }
            if (destination == null) destination = new List<T>();
            var count = source.Count;
            for (int i = 0; i < count; i++) {
                var token = source[i];
                var obj = new T();
                if (token != null) {
                    obj = context.Mapper.Map(token, obj);
                }
                destination.Add(obj);
            }
            return destination;
        }
    }

    // This is now redundant in favour of the Generic class above
    public class JArrayToExampleModelConverter : ITypeConverter<JArray, List<ExampleModel>>
    {
        public List<ExampleModel> Convert(JArray source, List<ExampleModel> destination, ResolutionContext context)
        {
            if (source == null) {
                throw new System.ArgumentNullException(nameof(source), $"Cannot Map from a null ExampleModel");
            }
            if (destination == null) destination = new List<ExampleModel>();
            var count = source.Count;
            for (int i = 0; i < count; i++) {
                var token = source[i];
                var obj = new ExampleModel();
                if (token != null) {
                    obj = context.Mapper.Map(token, obj);
                }
                destination.Add(obj);
            }
            return destination;
        }
    }
}

