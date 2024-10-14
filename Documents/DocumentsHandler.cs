using System;

namespace Librac.Documents
{
    public static class DocumentsHandler
    {
        private static readonly XmlToJson _xmlToJson = new XmlToJson();

        /// <summary>
        /// Converts xml document to json 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ConvertXmlToJson(string file)
        {
            try
            {
                return _xmlToJson.Convert(file);
            }
            catch (Exception ex)
            { 
                return $"Error while parsing the document: {ex.Message}";
            }
        }
    }
}
