using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FXml
    {
        public XmlNode Root { get; private set; }
        public List<FLField> Fields { get; private set; }

        public FXml()
        {
        }

        public FXml(string content)
        {
            Load(content);
        }

        public FXml(Assembly assemply, string path)
        {
            Load(assemply, path);
        }

        public void Load(string content)
        {
            try
            {
                var xmldoc = new XmlDocument();
                xmldoc.LoadXml(content);
                Root = xmldoc.DocumentElement;
                Fields = GetFields(Root);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        public void Load(Assembly assemply, string path)
        {
            try
            {
                var xmldoc = new XmlDocument();
                xmldoc.Load(assemply.GetManifestResourceStream(path));
                Root = xmldoc.DocumentElement;
                Fields = GetFields(Root);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        public FLGuide GetGuideContent()
        {
            if (Root == null)
                return null;
            var body = Root["body"].InnerText;
            Fields.ForEach(x => body = body.Replace($"[{x.Name}]", x.Header));
            var guide = new FLGuide();
            guide.Icon = GetAttribute(Root["header"], "icon", "CommentQuestionOutline");
            guide.Color = GetAttribute(Root["header"], "color", "#ffffff");
            guide.Title = GetAttribute(Root["header"], FSetting.Language.ToLower());
            guide.Body = body;
            return guide;
        }

        private List<FLField> GetFields(XmlNode xmlNode)
        {
            var result = new List<FLField>();
            if (xmlNode == null) return result;
            var fieldsNode = xmlNode["fields"];
            if (fieldsNode == null) return result;
            foreach (XmlNode child in fieldsNode.ChildNodes)
            {
                if (child.Name == "field")
                    result.Add(GetField(child));
            }
            return result;
        }

        private FLField GetField(XmlNode node)
        {
            if (node == null)
                return null;
            return new FLField(GetAttribute(node, "name"), GetAttribute(node["header"], FSetting.Language.ToLower()));
        }

        private string GetAttribute(XmlNode node, string name, string defaultValue = "")
        {
            if (node == null)
                return defaultValue;
            if (node.Attributes[name] is XmlAttribute attribute)
                return attribute.Value;
            return node[name] is XmlNode othNode ? othNode.InnerText : defaultValue;
        }

        public class FLGuide
        {
            public string Icon { get; set; }
            public string Color { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
        }

        public class FLField
        {
            public string Name { get; set; }
            public string Header { get; set; }

            public FLField()
            {
            }

            public FLField(string name, string header)
            {
                Name = name;
                Header = header;
            }
        }
    }
}