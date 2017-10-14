namespace YourGame.Client
{
    using System.IO;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    public static class Serialization
    {
        public static T DeepClone<T>(this T obj)
        {
            object data = obj.ToBson().FromBson<T>();
            return (T)data;
        }

        public static T FromBson<T>(this byte[] data)
        {
            var stream = new MemoryStream(data);
            stream.Seek(0, SeekOrigin.Begin);
            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
            serializer.Converters.Add(new WarsmithsClientConverter());
            var reader = new BsonReader(stream);
            return serializer.Deserialize<T>(reader);
        }

        public static byte[] ToBson(this object t)
        {
            var stream = new MemoryStream();

            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
            var writer = new BsonWriter(stream);
            serializer.Serialize(writer, t);
            return stream.ToArray();
        }

        public static T FromJson<T>(this string data)
        {
            var serializer = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.DeserializeObject<T>(data, serializer);
        }

        public static string ToJson(this object t)
        {
            var serializer = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.SerializeObject(t, serializer);
        }
    }
}
