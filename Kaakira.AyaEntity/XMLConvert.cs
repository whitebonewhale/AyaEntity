using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace WebConsole.AyaEntity
{
	public class XMLConvert
	{
		public static XElement ToElement<T>(string tag, T obj)
		{
			Type type = typeof(T);
			XElement element = new XElement(tag,
				from property in type.GetProperties()
				select new XElement(property.Name, property.GetValue(obj)));
			return element;
		}


		public static T Deserialize<T>(XElement element) where T : new()
		{
			Type type = typeof(T);
			
			PropertyInfo[] properties = typeof(T).GetProperties();
			T obj = new T();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(obj, element.Element(property.Name).Value);
			}
			return obj;

		}
	}
}
