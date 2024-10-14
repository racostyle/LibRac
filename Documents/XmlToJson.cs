using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace Librac.Documents
{
    internal class XmlToJson
    {
        internal string Convert(string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            var jsonObject = new Dictionary<string, object>();
            ProcessElement(doc.Root, jsonObject);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return System.Text.Json.JsonSerializer.Serialize(jsonObject, options);
        }

        private void ProcessElement(XElement element, Dictionary<string, object> parentObject)
        {
            var hasSimpleTextContent = !element.HasElements && !element.HasAttributes && !string.IsNullOrEmpty(element.Value.Trim());

            // If the element has simple text content and no attributes, add it directly.
            if (hasSimpleTextContent)
            {
                parentObject[element.Name.LocalName] = element.Value.Trim();
                return; // Stop further processing since it's just a text node.
            }

            // Handle attributes and complex structures
            var elementObject = new Dictionary<string, object>();
            foreach (var attribute in element.Attributes())
            {
                elementObject[$"@{attribute.Name.LocalName}"] = attribute.Value;
            }

            var childGroup = element.Elements().GroupBy(x => x.Name.LocalName).ToList();
            foreach (var group in childGroup)
            {
                if (group.Count() > 1) // Multiple elements with the same name should be an array
                {
                    var childList = new List<object>();
                    foreach (var childElement in group)
                    {
                        var childObject = new Dictionary<string, object>();
                        ProcessElement(childElement, childObject);
                        childList.Add(childObject.Values.Single()); // Add the single value or object
                    }
                    elementObject[group.Key] = childList;
                }
                else // Single element
                {
                    var childObject = new Dictionary<string, object>();
                    ProcessElement(group.Single(), childObject);
                    elementObject[group.Key] = childObject.Values.Single(); // Directly add the single value or object
                }
            }

            // Add this element's data to the parent object
            parentObject[element.Name.LocalName] = elementObject;
        }
    }
}
