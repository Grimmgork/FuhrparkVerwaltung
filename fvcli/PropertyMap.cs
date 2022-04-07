using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace fvcli
{
	public class PropertyMap<T>
	{
		Dictionary<Type, List<Property<T>>> properties = new Dictionary<Type, List<Property<T>>>();

		public void RegisterProperty(Type type, string propertyName, T metadata)
		{
			if(!properties.ContainsKey(type))
				properties.Add(type, new List<Property<T>>());

			properties[type].Add(new Property<T>(metadata, type.GetProperty(propertyName)));
		}
		
		public Property<T>[] GetProperties(Type type)
		{
			IEnumerable<Property<T>> result = new List<Property<T>>();

			while(properties.ContainsKey(type)){
				result = properties[type].Concat(result);
				type = type.BaseType;
			}

			return result.ToArray();
		}
	}

	public class Property<T>
	{
		private readonly PropertyInfo TargetProperty;

		public Type PropertyType { get => TargetProperty.PropertyType; }
		public Type ReflectedType { get => TargetProperty.ReflectedType; }
		public readonly T MetaData;

		internal Property(T metaData, PropertyInfo property)
		{
			this.MetaData = metaData;
			this.TargetProperty = property;
		}

		public void SetValue(object o, object value)
		{
			TargetProperty.SetValue(o, value);
		}

		public object GetValue(object o)
		{
			return TargetProperty.GetValue(o);
		}
	}
}