using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Librac.ConfigurationsLib
{
    internal class ConfigurationsMethods
    {
        #region LOADING
        internal Dictionary<string, string>? LoadConfigurationFromFile(string fileLocation)
        {
            var task = Task.Run(async () => await LoadConfigAsync(fileLocation));
            task.Wait();
            return task.Result;
        }

        private async Task<Dictionary<string,string>?> LoadConfigAsync(string fileLocation)
        {
            if (File.Exists(fileLocation))
            {
                await using var stream = File.OpenRead(fileLocation);
                try
                {
                    using (JsonDocument doc = await JsonDocument.ParseAsync(stream))
                    {
                        var dict = new Dictionary<string, string>();
                        FillDictionaryFromJsonElement(doc.RootElement, dict, string.Empty);
                        return dict;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return null;
        }

        private void FillDictionaryFromJsonElement(JsonElement element, Dictionary<string, string> dict, string prefix)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        string key = !string.IsNullOrEmpty(prefix) ? $"{prefix}:{property.Name}" : property.Name;
                        FillDictionaryFromJsonElement(property.Value, dict, key);
                    }
                    break;
                case JsonValueKind.Array:
                    int index = 0;
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        string key = $"{prefix}[{index++}]";
                        FillDictionaryFromJsonElement(item, dict, key);
                    }
                    break;
                default:
                    dict[prefix] = element.ToString();
                    break;
            }
        }
        #endregion

        #region SAVING
        internal void SaveConfigurationToFile(Dictionary<string, string> dict, string fileLocation)
        {
            JsonNode jsonObject = CreateJsonObjectFromNestedDictionary(dict);
            string jsonText = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = false });
            File.WriteAllText(fileLocation, jsonText);
        }

        private JsonNode CreateJsonObjectFromNestedDictionary(Dictionary<string, string> nestedDictionary)
        {
            JsonObject jObject = new JsonObject();

            foreach (var kvp in nestedDictionary)
            {
                try
                {
                    var keys = kvp.Key.Split(':'); // Split keys by ':'
                    JsonNode currentJsonNode = jObject;

                    for (int i = 0; i < keys.Length - 1; i++)
                    {
                        var key = keys[i];

                        if (currentJsonNode[key] == null)
                        {
                            var nested = new JsonObject();
                            currentJsonNode[key] = nested;
                            currentJsonNode = nested;
                        }
                        else
                        {
                            currentJsonNode = currentJsonNode[key];
                        }
                    }

                    // Set the value at the appropriate key
                    currentJsonNode[keys[keys.Length - 1]] = kvp.Value;
                } catch { }
            }
            return jObject;
        }
        #endregion
    }
}
