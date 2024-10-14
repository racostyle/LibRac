using System;
using System.Collections.Generic;

namespace Librac.Documents
{
    public static class DocumentsHandler
    {
        private static readonly XmlToJson _xmlToJson = new XmlToJson();

        /// <summary>
        /// Converts xml document to json 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filters">for example providing "xmlns" will exclude schemas headers at the start</param>
        /// <returns></returns>
        public static string ConvertXmlToJson(string file, List<string> filters)
        {
            try
            {
                return _xmlToJson.Convert(file, filters);
            }
            catch (Exception ex)
            { 
                return $"Error while parsing the document: {ex.Message}";
            }
        }
    }
}
