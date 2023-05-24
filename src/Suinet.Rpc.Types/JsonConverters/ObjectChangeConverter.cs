using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace Suinet.Rpc.Types.JsonConverters
{
    public class ObjectChangeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectChange).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            switch (item["type"].Value<string>())
            {
                case "published":
                    return item.ToObject<PublishedObjectChange>(serializer);
                case "transferred":
                    return item.ToObject<TransferredObjectChange>(serializer);
                case "mutated":
                    return item.ToObject<MutatedObjectChange>(serializer);
                case "deleted":
                    return item.ToObject<DeletedObjectChange>(serializer);
                case "wrapped":
                    return item.ToObject<WrappedObjectChange>(serializer);
                case "created":
                    return item.ToObject<CreatedObjectChange>(serializer);
                default:
                    throw new InvalidOperationException("Type field not found.");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
