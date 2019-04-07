using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x0200000E RID: 14
public static class HGXml
{
	// Token: 0x0600003D RID: 61 RVA: 0x00003CED File Offset: 0x00001EED
	public static void Register<T>(HGXml.Serializer<T> serializer, HGXml.Deserializer<T> deserializer)
	{
		HGXml.SerializationRules<T>.defaultRules = new HGXml.SerializationRules<T>
		{
			serializer = serializer,
			deserializer = deserializer
		};
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00003D07 File Offset: 0x00001F07
	public static void RegisterEnum<T>() where T : struct
	{
		HGXml.SerializationRules<T>.defaultRules = new HGXml.SerializationRules<T>
		{
			serializer = new HGXml.Serializer<T>(HGXml.<>c__4<T>.<>9.<RegisterEnum>g__Serializer|4_0),
			deserializer = new HGXml.Deserializer<T>(HGXml.<>c__4<T>.<>9.<RegisterEnum>g__Deserializer|4_1)
		};
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00003D3F File Offset: 0x00001F3F
	[NotNull]
	public static XElement ToXml<T>(string name, T value)
	{
		return HGXml.ToXml<T>(name, value, HGXml.SerializationRules<T>.defaultRules);
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00003D50 File Offset: 0x00001F50
	[NotNull]
	public static XElement ToXml<T>(string name, T value, HGXml.SerializationRules<T> rules)
	{
		XElement xelement = new XElement(name);
		rules.serializer(xelement, value);
		return xelement;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00003D77 File Offset: 0x00001F77
	public static bool FromXml<T>([NotNull] XElement element, ref T value)
	{
		return HGXml.FromXml<T>(element, ref value, HGXml.SerializationRules<T>.defaultRules);
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00003D85 File Offset: 0x00001F85
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

	// Token: 0x06000043 RID: 67 RVA: 0x00003DBB File Offset: 0x00001FBB
	public static bool FromXml<T>([NotNull] XElement element, [NotNull] Action<T> setter)
	{
		return HGXml.FromXml<T>(element, setter, HGXml.SerializationRules<T>.defaultRules);
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00003DCC File Offset: 0x00001FCC
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

	// Token: 0x06000045 RID: 69 RVA: 0x00003DF8 File Offset: 0x00001FF8
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

	// Token: 0x06000046 RID: 70 RVA: 0x00003F98 File Offset: 0x00002198
	public static void Deserialize<T>(this XElement element, ref T dest)
	{
		HGXml.FromXml<T>(element, ref dest);
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00003FA2 File Offset: 0x000021A2
	public static void Deserialize<T>(this XElement element, ref T dest, HGXml.SerializationRules<T> rules)
	{
		HGXml.FromXml<T>(element, ref dest, rules);
	}

	// Token: 0x0200000F RID: 15
	// (Invoke) Token: 0x06000049 RID: 73
	public delegate void Serializer<T>(XElement element, T contents);

	// Token: 0x02000010 RID: 16
	// (Invoke) Token: 0x0600004D RID: 77
	public delegate bool Deserializer<T>(XElement element, ref T contents);

	// Token: 0x02000011 RID: 17
	public class SerializationRules<T>
	{
		// Token: 0x0400005F RID: 95
		public HGXml.Serializer<T> serializer;

		// Token: 0x04000060 RID: 96
		public HGXml.Deserializer<T> deserializer;

		// Token: 0x04000061 RID: 97
		public static HGXml.SerializationRules<T> defaultRules;
	}
}
