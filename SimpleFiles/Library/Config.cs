using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SimpleFiles.Library
{
    public static class Config
    {
        public static string Password
        {
            get { return Get("Password"); }
            set { Set("Password", value); }
        }

        private static void CreateConfig()
        {
            if (!Directory.Exists(ConfigFolderPath))
                Directory.CreateDirectory(ConfigFolderPath);

            if (!File.Exists(ConfigFilePath) || (new FileInfo(ConfigFilePath).Length < ("Config").Length))
            {
                var xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t",
                    NewLineOnAttributes = true
                };

                using (var writer = XmlWriter.Create(ConfigFilePath, xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Config");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        public static void Set(string name, string value)
        {
            CreateConfig();

            var doc = new XmlDocument();
            doc.Load(ConfigFilePath);

            XmlNode root = doc.DocumentElement;

            if (root != null)
            {
                var settingNode = root.SelectSingleNode("descendant::" + name);

                if (settingNode != null)
                    settingNode.InnerText = value;
                else
                {
                    var currNode = doc.SelectSingleNode("/Config");
                    var element = doc.CreateNode(XmlNodeType.Element, name, null);
                    element.InnerText = value;
                    currNode.InsertAfter(element, currNode.LastChild);
                }

                doc.Save(ConfigFilePath);
            }
        }

        /// <summary>
        /// Accessor for a specific config property.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Value of config property if it exists; null if it doesn't exist.</returns>
        public static string Get(string name)
        {
            CreateConfig();

            var xDoc = XDocument.Load(ConfigFilePath);

            var element = (from c in xDoc.Root.Elements(name)
                select c).SingleOrDefault();

            string value = null;
            if (element != null)
                value = element.Value;

            return value;
        }

        public static readonly string ConfigFolderPath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        public static readonly string ConfigFilePath = ConfigFolderPath + @"\Config.xml";
    }
}