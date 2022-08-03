using Newtonsoft.Json;

namespace ACE.Plugin.Web
{
    internal static class Util
    {
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, serializationSettings);
        }
        public static T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json);
        private static readonly JsonSerializerSettings serializationSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        };
    }
}
