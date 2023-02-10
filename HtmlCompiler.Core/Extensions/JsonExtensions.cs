using Newtonsoft.Json.Linq;

namespace HtmlCompiler.Core.Extensions;

public static class JsonExtensions
{
    public static string UpdateJsonProperty(this string json, string key, object value)
    {
        // Parse the input string into a JObject
        JObject jObject = JObject.Parse(json);

        // Split the key into parts using the ':' separator
        string[] parts = key.Split(':');

        // Traverse the JObject to find the property
        JToken current = jObject;
        for (int i = 0; i < parts.Length - 1; i++)
        {
            string part = parts[i];
            JToken next = current[part]!;
            if (next == null)
            {
                // Create a new JObject if the property doesn't exist
                next = new JObject();
                ((JObject)current).Add(part, next);
            }
            current = next;
        }

        // Update or add the final property
        string lastPart = parts[parts.Length - 1];
        if (current[lastPart] != null)
        {
            current[lastPart] = JToken.FromObject(value);
        }
        else
        {
            ((JObject)current).Add(lastPart, JToken.FromObject(value));
        }

        // Serialize the JObject back to a string and return it
        return jObject.ToString();
    }

}