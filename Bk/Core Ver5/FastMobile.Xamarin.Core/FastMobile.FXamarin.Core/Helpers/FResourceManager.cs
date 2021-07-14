using System.Reflection;
using System.Xml;
using Xamarin.Essentials;

namespace FastMobile.FXamarin.Core
{
    public class FResourceManager
    {
        public static Assembly Assembly { get; }

        private XmlNode Controller;

        static FResourceManager()
        {
            Assembly = typeof(FResourceManager).Assembly;
        }

        public FResourceManager()
        {
        }

        public FResourceManager(string filePath, string controller)
        {
            Init(Assembly, filePath, controller);
        }

        public FResourceManager(Assembly assembly, string filePath, string controller)
        {
            Init(assembly, filePath, controller);
        }

        public void Init(Assembly assembly, string filePath, string controller)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(assembly.GetManifestResourceStream(filePath));
            Controller = xmldoc.DocumentElement.SelectSingleNode($"/controllers/controller[@name='{controller}']");
        }

        public string GetString(string name, string defaultValue = "", string attribute = "")
        {
            var node = Controller.SelectSingleNode($"fields/field[@name='{name}']/header{DeviceInfo.Platform}");
            if (node == null) node = Controller.SelectSingleNode($"fields/field[@name='{name}']/header");
            if (node == null) node = Controller.SelectSingleNode($"fields/field[@name='100']/header");
            return GetAttribute(node, string.IsNullOrEmpty(attribute) ? FSetting.Language.ToLower() : attribute.ToLower(), defaultValue);
        }

        private string GetAttribute(XmlNode node, string name, string defaultValue = "")
        {
            if (node == null)
                return defaultValue;
            if (node.Attributes[name] is XmlAttribute attribute)
                return attribute.Value;
            return node[name] is XmlNode othNode ? othNode.InnerText : defaultValue;
        }
    }
}