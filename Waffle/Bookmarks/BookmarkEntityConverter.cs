using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Waffle.Bookmarks
{
    public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(BookmarkEntity).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            {
                return null;
            }

            return base.ResolveContractConverter(objectType);
        }
    }

    public class BookmarkEntityConverter : JsonConverter
    {
        static readonly JsonSerializerSettings SpecifiedSubclassConversion = new() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BookmarkEntity);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            return jObject["BookmarkEntityType"].Value<string>() switch
            {
                "Bookmark" => JsonConvert.DeserializeObject<Bookmark>(jObject.ToString(), SpecifiedSubclassConversion),
                "BookmarkFolder" => JsonConvert.DeserializeObject<BookmarkFolder>(jObject.ToString(), SpecifiedSubclassConversion),
                _ => throw new InvalidOperationException(),
            };

            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
}
