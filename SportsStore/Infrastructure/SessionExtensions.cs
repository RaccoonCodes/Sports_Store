using System.Text.Json;

//These methods serialize objects into the JavaScript Object Notation format,
//making it easy to store and retrieve Cart objects

namespace SportsStore.Infrastructure
{
    public static class SessionExtensions
    {
        //extetsion method for Isession interface. Takes key and object value
        // serializes the object to JSON and stores it in session
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        // retrieves JSON string from session based on key. if its not 
        // null, deserialize it. else return default value for type T. 
        public static T? GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            return sessionData == null
            ? default(T) : JsonSerializer.Deserialize<T>(sessionData);
        }
    }
}