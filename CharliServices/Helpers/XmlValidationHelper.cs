using McpNetwork.Charli.Server.Exceptions;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace McpNetwork.Charli.Environment.Helpers
{
    internal static class XmlValidationHelper
    {
        private const string xsdResourcesPath = "McpNetwork.Charli.Server.xsd";
        private static XmlSchemaSet schemaSet = null;
        private static XmlReaderSettings xmlSettings = null;

        public static bool ValidateDocument(string filename)
        {
            bool fctResult = true;
            try
            {
                if (schemaSet == null)
                {
                    XmlValidationHelper.LoadSystemValidators();
                }

                XmlReader reader = XmlReader.Create(filename, XmlValidationHelper.xmlSettings);
                XmlDocument document = new();
                document.Load(reader);

                ValidationEventHandler eventHandler = new(ValidationEventHandler);
                try
                {
                    document.Validate(eventHandler);
                }
                catch (Exception e)
                {
                    LogError(e);
                    fctResult = false;
                }
                reader.Close();
            }
            catch (Exception e)
            {
                fctResult = false;
                LogError(e);
            }

            return fctResult;
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    throw new CharliException(e.Message);
                case XmlSeverityType.Warning:
                    LogWarning(e.Message);
                    break;
            }
        }

        public static bool LoadDocument<T>(string filename, out T result) where T : new()
        {
            bool fctResult = true;
            result = new T();
            try
            {

                var serializer = new XmlSerializer(result.GetType());
                using (var reader = new FileStream(filename, FileMode.Open))
                {
                    result = (T)serializer.Deserialize(reader);

                }
            }
            catch (Exception e)
            {
                fctResult = false;
                LogError(e);
            }
            return (fctResult);
        }

        public static T LoadDocument<T>(string filename) where T : new()
        {
            var result = new T();
            try
            {
                var serializer = new XmlSerializer(result.GetType());
                var fileContent = File.ReadAllText(filename);  
                using (var reader = new FileStream(filename, FileMode.Open))
                {
                    result = (T)serializer.Deserialize(reader);

                }
            }
            catch (Exception e)
            {
                result = default;
                LogError(e);
            }
            return (result);
        }

        internal static T LoadFromContent<T>(string content) where T : new()
        {
            var result = default(T);
            try
            {
                var serializer = new XmlSerializer(result.GetType());
                using (var reader = new StringReader(content))
                {
                    result = (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                result = default;
                LogError(e);
            }
            return (result);
        }

        private static XmlSchema GetSystemValidationSchema(string resourceName)
        {
            XmlSchema? result = null;
            resourceName = string.Format("{0}.{1}", xsdResourcesPath, resourceName);
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                result = XmlSchema.Read(stream, null);
            }
            return result;
        }

        private static void LoadSystemValidators()
        {
            XmlValidationHelper.schemaSet = new XmlSchemaSet
            {
                XmlResolver = null
            };
            XmlValidationHelper.schemaSet.Add(XmlValidationHelper.GetSystemValidationSchema("settings.xsd"));
            XmlValidationHelper.schemaSet.Add(XmlValidationHelper.GetSystemValidationSchema("localization.xsd"));
            XmlValidationHelper.schemaSet.Compile();

            XmlValidationHelper.xmlSettings = new XmlReaderSettings();
            XmlValidationHelper.xmlSettings.Schemas.Add(XmlValidationHelper.GetSystemValidationSchema("settings.xsd"));
            XmlValidationHelper.xmlSettings.Schemas.Add(XmlValidationHelper.GetSystemValidationSchema("localization.xsd"));
            XmlValidationHelper.xmlSettings.ValidationType = ValidationType.Schema;
        }

        #region Logs
        private static void LogError(Exception e)
        {
            // TODO --> Where is the logger ?
            //CharliLogger.LogError(ELoggerSystem.XmlValidator.ToString(), e);
        }

        private static void LogWarning(string message)
        {
            // TODO --> Where is the logger ?
            //CharliLogger.LogError(ELoggerSystem.XmlValidator.ToString(), message);
        }
        #endregion

    }
}
