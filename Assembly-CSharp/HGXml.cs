using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x0200005E RID: 94
public static class HGXml
{
	// Token: 0x06000186 RID: 390 RVA: 0x00008D75 File Offset: 0x00006F75
	public static void Register<T>(HGXml.Serializer<T> serializer, HGXml.Deserializer<T> deserializer)
	{
		HGXml.SerializationRules<T>.defaultRules = new HGXml.SerializationRules<T>
		{
			serializer = serializer,
			deserializer = deserializer
		};
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00008D8F File Offset: 0x00006F8F
	public static void RegisterEnum<T>() where T : struct
	{
		HGXml.SerializationRules<T>.defaultRules = new HGXml.SerializationRules<T>
		{
			serializer = new HGXml.Serializer<T>(HGXml.<>c__4<T>.<>9.<RegisterEnum>g__Serializer|4_0),
			deserializer = new HGXml.Deserializer<T>(HGXml.<>c__4<T>.<>9.<RegisterEnum>g__Deserializer|4_1)
		};
	}

	// Token: 0x06000188 RID: 392 RVA: 0x00008DC7 File Offset: 0x00006FC7
	[NotNull]
	public static XElement ToXml<T>(string name, T value)
	{
		return HGXml.ToXml<T>(name, value, HGXml.SerializationRules<T>.defaultRules);
	}

	// Token: 0x06000189 RID: 393 RVA: 0x00008DD8 File Offset: 0x00006FD8
	[NotNull]
	public static XElement ToXml<T>(string name, T value, HGXml.SerializationRules<T> rules)
	{
		XElement xelement = new XElement(name);
		rules.serializer(xelement, value);
		return xelement;
	}

	// Token: 0x0600018A RID: 394 RVA: 0x00008DFF File Offset: 0x00006FFF
	public static bool FromXml<T>([NotNull] XElement element, ref T value)
	{
		return HGXml.FromXml<T>(element, ref value, HGXml.SerializationRules<T>.defaultRules);
	}

	// Token: 0x0600018B RID: 395 RVA: 0x00008E0D File Offset: 0x0000700D
	public static bool FromXml<T>([NotNull] XElement element, ref T value, HGXml.SerializationRules<T> rules)
	{
		if (rules == null)
		{
			Debug.LogFormat("Serialization rules not defined for type <{0}>", new object[]
			{
				typeof(T).Name
			});
			return false;
		}
		return rules.deserializer(element, ref value);
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00008E43 File Offset: 0x00007043
	public static bool FromXml<T>([NotNull] XElement element, [NotNull] Action<T> setter)
	{
		return HGXml.FromXml<T>(element, setter, HGXml.SerializationRules<T>.defaultRules);
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00008E54 File Offset: 0x00007054
	public static bool FromXml<T>([NotNull] XElement element, [NotNull] Action<T> setter, [NotNull] HGXml.SerializationRules<T> rules)
	{
		T obj = default(T);
		if (HGXml.FromXml<T>(element, ref obj, rules))
		{
			setter(obj);
			return true;
		}
		return false;
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00008E80 File Offset: 0x00007080
	static HGXml()
	{
		HGXml.Register<int>(delegate(XElement element, int contents)
		{
			element.Value = TextSerialization.ToStringInvariant(contents);
		}, delegate(XElement element, ref int contents)
		{
			int num;
			if (TextSerialization.TryParseInvariant(element.Value, out num))
			{
				contents = num;
				return true;
			}
			return false;
		});
		HGXml.Register<uint>(delegate(XElement element, uint contents)
		{
			element.Value = TextSerialization.ToStringInvariant(contents);
		}, delegate(XElement element, ref uint contents)
		{
			uint num;
			if (TextSerialization.TryParseInvariant(element.Value, out num))
			{
				contents = num;
				return true;
			}
			return false;
		});
		HGXml.Register<ulong>(delegate(XElement element, ulong contents)
		{
			element.Value = TextSerialization.ToStringInvariant(contents);
		}, delegate(XElement element, ref ulong contents)
		{
			ulong num;
			if (TextSerialization.TryParseInvariant(element.Value, out num))
			{
				contents = num;
				return true;
			}
			return false;
		});
		HGXml.Register<bool>(delegate(XElement element, bool contents)
		{
			element.Value = (contents ? "1" : "0");
		}, delegate(XElement element, ref bool contents)
		{
			int num;
			if (TextSerialization.TryParseInvariant(element.Value, out num))
			{
				contents = (num != 0);
				return true;
			}
			return false;
		});
		HGXml.Register<float>(delegate(XElement element, float contents)
		{
			element.Value = TextSerialization.ToStringInvariant(contents);
		}, delegate(XElement element, ref float contents)
		{
			float num;
			if (TextSerialization.TryParseInvariant(element.Value, out num))
			{
				contents = num;
				return true;
			}
			return false;
		});
		HGXml.Register<double>(delegate(XElement element, double contents)
		{
			element.Value = TextSerialization.ToStringInvariant(contents);
		}, delegate(XElement element, ref double contents)
		{
			double num;
			if (TextSerialization.TryParseInvariant(element.Value, out num))
			{
				contents = num;
				return true;
			}
			return false;
		});
		HGXml.Register<string>(delegate(XElement element, string contents)
		{
			element.Value = contents;
		}, delegate(XElement element, ref string contents)
		{
			contents = element.Value;
			return true;
		});
		MethodInfo method = typeof(HGXml).GetMethod("RegisterEnum");
		foreach (TypeInfo typeInfo in from t in typeof(HGXml).Assembly.DefinedTypes
		where t.IsEnum
		select t)
		{
			method.MakeGenericMethod(new Type[]
			{
				typeInfo
			}).Invoke(null, Array.Empty<object>());
		}
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00009020 File Offset: 0x00007220
	public static void Deserialize<T>(this XElement element, ref T dest)
	{
		HGXml.FromXml<T>(element, ref dest);
	}

	// Token: 0x06000190 RID: 400 RVA: 0x0000902A File Offset: 0x0000722A
	public static void Deserialize<T>(this XElement element, ref T dest, HGXml.SerializationRules<T> rules)
	{
		HGXml.FromXml<T>(element, ref dest, rules);
	}

	// Token: 0x0200005F RID: 95
	// (Invoke) Token: 0x06000192 RID: 402
	public delegate void Serializer<T>(XElement element, T contents);

	// Token: 0x02000060 RID: 96
	// (Invoke) Token: 0x06000196 RID: 406
	public delegate bool Deserializer<T>(XElement element, ref T contents);

	// Token: 0x02000061 RID: 97
	public class SerializationRules<T>
	{
		// Token: 0x040001B0 RID: 432
		public HGXml.Serializer<T> serializer;

		// Token: 0x040001B1 RID: 433
		public HGXml.Deserializer<T> deserializer;

		// Token: 0x040001B2 RID: 434
		public static HGXml.SerializationRules<T> defaultRules;
	}
}
