namespace YourGame.Client
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using YourGame.Common.Domain;

    public class ClientConverter : JsonConverter
    {
        private readonly Type[] Types;

        public ClientConverter()
        {
            Types = Assembly.Load("YourGame.Common").GetTypes();
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IEntity);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            JObject jo;
            try
            {
                jo = JObject.Load(reader);
            }
            catch
            {
                return null;
            }

            var values = jo["_t"].Values();

            if (!values.Any())
            {
                var val = jo["_t"].Value<string>();
                var specificType = Types.FirstOrDefault(t => t.Name == val);

                var firedObject = jo.ToObject(specificType);
                return firedObject;
            }

            var lastValue = values.Last();
            if (lastValue != null)
            {
                var val = lastValue.Value<string>();

                var specificType = Types.FirstOrDefault(t => t.Name == val);

                var firedObject = jo.ToObject(specificType);
                return firedObject;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}