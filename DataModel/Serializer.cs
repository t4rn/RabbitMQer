using Newtonsoft.Json;

namespace DataModel
{
    public static class Serializer
    {
        public static string SerializeToJson(object obj)
        {
            if (obj != null)
            {
                return JsonConvert.SerializeObject(obj);
            }
            else
            {
                return null;
            }
        }

        public static T DeserializeFromJson<T>(string json)
        {
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }
    }
}
