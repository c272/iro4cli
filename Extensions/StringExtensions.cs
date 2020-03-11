using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace iro4cli
{
	/// <summary>
    /// Extensions for strings in Iro.
    /// </summary>
    public static class StringExtensions
    {
		public static string FormatForXML(this string unescaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }
    }
}
