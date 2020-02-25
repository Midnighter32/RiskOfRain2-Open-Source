using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntityStates;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000012 RID: 18
[CreateAssetMenu]
public class EntityStateManager : ScriptableObject, ISerializationCallbackReceiver
{
	// Token: 0x06000056 RID: 86 RVA: 0x000040A0 File Offset: 0x000022A0
	private EntityStateManager.StateInfo GetStateInfo(Type stateType)
	{
		if (stateType == null || !stateType.IsSubclassOf(typeof(EntityState)))
		{
			return null;
		}
		EntityStateManager.StateInfo stateInfo = this.stateInfoList.Find((EntityStateManager.StateInfo currentItem) => currentItem.serializedType.stateType == stateType);
		if (stateInfo == null)
		{
			stateInfo = new EntityStateManager.StateInfo();
			stateInfo.SetStateType(stateType);
			this.stateInfoList.Add(stateInfo);
		}
		return stateInfo;
	}

	// Token: 0x06000057 RID: 87 RVA: 0x0000411C File Offset: 0x0000231C
	private void ApplyStatic()
	{
		foreach (EntityStateManager.StateInfo stateInfo in this.stateInfoList)
		{
			stateInfo.ApplyStatic();
		}
	}

	// Token: 0x06000058 RID: 88 RVA: 0x0000416C File Offset: 0x0000236C
	public void Initialize()
	{
		this.ApplyStatic();
		this.GenerateInstanceFieldInitializers();
	}

	// Token: 0x06000059 RID: 89 RVA: 0x0000417C File Offset: 0x0000237C
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		foreach (EntityStateManager.StateInfo stateInfo2 in this.stateInfoList)
		{
			stateInfo2.RefreshStateType();
		}
		this.stateInfoList.RemoveAll((EntityStateManager.StateInfo stateInfo) => !stateInfo.IsValid());
	}

	// Token: 0x0600005A RID: 90 RVA: 0x000041F8 File Offset: 0x000023F8
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		foreach (EntityStateManager.StateInfo stateInfo in this.stateInfoList)
		{
			stateInfo.RefreshStateType();
		}
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00004248 File Offset: 0x00002448
	private void GenerateInstanceFieldInitializers()
	{
		EntityStateManager.instanceFieldInitializers.Clear();
		foreach (EntityStateManager.StateInfo stateInfo in this.stateInfoList)
		{
			Type stateType = stateInfo.serializedType.stateType;
			if (!(stateType == null))
			{
				Action<EntityState> action = stateInfo.GenerateInstanceFieldInitializerDelegate();
				if (action != null)
				{
					EntityStateManager.instanceFieldInitializers.Add(stateType, action);
				}
			}
		}
	}

	// Token: 0x0600005C RID: 92 RVA: 0x000042CC File Offset: 0x000024CC
	public static void InitializeStateFields(EntityState entityState)
	{
		Action<EntityState> action;
		EntityStateManager.instanceFieldInitializers.TryGetValue(entityState.GetType(), out action);
		if (action != null)
		{
			action(entityState);
		}
	}

	// Token: 0x04000079 RID: 121
	[SerializeField]
	private List<EntityStateManager.StateInfo> stateInfoList = new List<EntityStateManager.StateInfo>();

	// Token: 0x0400007A RID: 122
	[SerializeField]
	[HideInInspector]
	private string endMarker = "GIT_END";

	// Token: 0x0400007B RID: 123
	private static readonly Dictionary<Type, Action<EntityState>> instanceFieldInitializers = new Dictionary<Type, Action<EntityState>>();

	// Token: 0x02000013 RID: 19
	[Serializable]
	public class GameObjectField
	{
		// Token: 0x0400007C RID: 124
		public string key;

		// Token: 0x0400007D RID: 125
		public GameObject value;
	}

	// Token: 0x02000014 RID: 20
	[Serializable]
	private class StateInfo
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00004328 File Offset: 0x00002528
		private static bool FieldHasSerializeAttribute(FieldInfo fieldInfo)
		{
			return fieldInfo.GetCustomAttributes(true).Any<SerializeField>();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004336 File Offset: 0x00002536
		private static bool FieldLacksNonSerializedAttribute(FieldInfo fieldInfo)
		{
			return !fieldInfo.GetCustomAttributes(true).Any<NonSerializedAttribute>();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004348 File Offset: 0x00002548
		public void SetStateType(Type stateType)
		{
			this.serializedType.stateType = stateType;
			stateType = this.serializedType.stateType;
			if (stateType == null)
			{
				return;
			}
			IEnumerable<FieldInfo> first = stateType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Where(new Func<FieldInfo, bool>(EntityStateManager.StateInfo.FieldHasSerializeAttribute));
			IEnumerable<FieldInfo> second = (from fieldInfo in stateType.GetFields(BindingFlags.Static | BindingFlags.Public)
			where fieldInfo.DeclaringType == stateType
			select fieldInfo).Where(new Func<FieldInfo, bool>(EntityStateManager.StateInfo.FieldLacksNonSerializedAttribute));
			List<FieldInfo> list = first.Concat(second).ToList<FieldInfo>();
			Dictionary<FieldInfo, EntityStateManager.StateInfo.Field> dictionary = new Dictionary<FieldInfo, EntityStateManager.StateInfo.Field>();
			using (List<FieldInfo>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FieldInfo fieldInfo = enumerator.Current;
					EntityStateManager.StateInfo.Field field = this.stateFieldList.Find((EntityStateManager.StateInfo.Field item) => item.GetFieldName() == fieldInfo.Name);
					if (field == null)
					{
						Debug.LogFormat("Could not find field {0}.{1}. Initializing new field.", new object[]
						{
							stateType.Name,
							fieldInfo.Name
						});
						field = new EntityStateManager.StateInfo.Field(fieldInfo.Name);
					}
					dictionary[fieldInfo] = field;
				}
			}
			this.stateFieldList.Clear();
			foreach (FieldInfo fieldInfo2 in list)
			{
				EntityStateManager.StateInfo.Field field2 = dictionary[fieldInfo2];
				field2.owner = this;
				field2.SetFieldInfo(fieldInfo2);
				this.stateFieldList.Add(field2);
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000451C File Offset: 0x0000271C
		public void RefreshStateType()
		{
			this.SetStateType(this.serializedType.stateType);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004530 File Offset: 0x00002730
		public void ApplyStatic()
		{
			Type stateType = this.serializedType.stateType;
			if (stateType != null)
			{
				foreach (EntityStateManager.StateInfo.Field field in this.stateFieldList)
				{
					FieldInfo field2 = stateType.GetField(field.GetFieldName(), BindingFlags.Static | BindingFlags.Public);
					if (field.MatchesFieldInfo(field2) && field2.IsStatic)
					{
						field.Apply(field2, null);
					}
				}
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000045BC File Offset: 0x000027BC
		public Action<EntityState> GenerateInstanceFieldInitializerDelegate()
		{
			Type stateType = this.serializedType.stateType;
			if (stateType == null)
			{
				return null;
			}
			List<EntityStateManager.StateInfo.FieldValuePair> list = new List<EntityStateManager.StateInfo.FieldValuePair>();
			for (int i = 0; i < this.stateFieldList.Count; i++)
			{
				EntityStateManager.StateInfo.Field field = this.stateFieldList[i];
				EntityStateManager.StateInfo.FieldValuePair fieldValuePair = new EntityStateManager.StateInfo.FieldValuePair
				{
					fieldInfo = stateType.GetField(field.GetFieldName(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy),
					value = field.valueAsSystemObject
				};
				if (!(fieldValuePair.fieldInfo == null))
				{
					list.Add(fieldValuePair);
				}
			}
			EntityStateManager.StateInfo.FieldValuePair[] fieldValuePairs = list.ToArray();
			if (fieldValuePairs.Length == 0)
			{
				return null;
			}
			return delegate(EntityState entityState)
			{
				for (int j = 0; j < fieldValuePairs.Length; j++)
				{
					EntityStateManager.StateInfo.FieldValuePair fieldValuePair2 = fieldValuePairs[j];
					fieldValuePair2.fieldInfo.SetValue(entityState, fieldValuePair2.value);
				}
			};
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000467C File Offset: 0x0000287C
		public EntityStateManager.StateInfo.Field FindField(string fieldName)
		{
			return this.stateFieldList.Find((EntityStateManager.StateInfo.Field value) => value.GetFieldName() == fieldName);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000046AD File Offset: 0x000028AD
		public bool IsValid()
		{
			return this.serializedType.stateType != null;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000046C0 File Offset: 0x000028C0
		public IList<EntityStateManager.StateInfo.Field> GetFields()
		{
			return this.stateFieldList.AsReadOnly();
		}

		// Token: 0x0400007E RID: 126
		public SerializableEntityStateType serializedType;

		// Token: 0x0400007F RID: 127
		[SerializeField]
		[FormerlySerializedAs("stateStaticFieldList")]
		private List<EntityStateManager.StateInfo.Field> stateFieldList = new List<EntityStateManager.StateInfo.Field>();

		// Token: 0x04000080 RID: 128
		private const BindingFlags defaultInstanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		// Token: 0x04000081 RID: 129
		private const BindingFlags defaultStaticBindingFlags = BindingFlags.Static | BindingFlags.Public;

		// Token: 0x02000015 RID: 21
		[Serializable]
		public class Field
		{
			// Token: 0x0600006A RID: 106 RVA: 0x000046E0 File Offset: 0x000028E0
			public Field(string fieldName)
			{
				this._fieldName = fieldName;
			}

			// Token: 0x0600006B RID: 107 RVA: 0x000046F0 File Offset: 0x000028F0
			public void SetFieldInfo(FieldInfo fieldInfo)
			{
				this._fieldName = fieldInfo.Name;
				EntityStateManager.StateInfo.Field.ValueType valueType = EntityStateManager.StateInfo.Field.ValueType.Invalid;
				Type fieldType = fieldInfo.FieldType;
				if (fieldType == typeof(int))
				{
					valueType = EntityStateManager.StateInfo.Field.ValueType.Int;
				}
				else if (fieldType == typeof(float))
				{
					valueType = EntityStateManager.StateInfo.Field.ValueType.Float;
				}
				else if (fieldType == typeof(string))
				{
					valueType = EntityStateManager.StateInfo.Field.ValueType.String;
				}
				else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
				{
					valueType = EntityStateManager.StateInfo.Field.ValueType.Object;
				}
				else if (fieldType == typeof(bool))
				{
					valueType = EntityStateManager.StateInfo.Field.ValueType.Bool;
				}
				else if (fieldType == typeof(AnimationCurve))
				{
					valueType = EntityStateManager.StateInfo.Field.ValueType.AnimationCurve;
				}
				else if (fieldType == typeof(Vector3))
				{
					valueType = EntityStateManager.StateInfo.Field.ValueType.Vector3;
				}
				if (this._valueType != valueType)
				{
					this.ResetValues();
					this._valueType = valueType;
				}
			}

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x0600006C RID: 108 RVA: 0x000047C3 File Offset: 0x000029C3
			public EntityStateManager.StateInfo.Field.ValueType valueType
			{
				get
				{
					return this._valueType;
				}
			}

			// Token: 0x17000010 RID: 16
			// (get) Token: 0x0600006D RID: 109 RVA: 0x000047CB File Offset: 0x000029CB
			public int intValue
			{
				get
				{
					return this._intValue;
				}
			}

			// Token: 0x17000011 RID: 17
			// (get) Token: 0x0600006E RID: 110 RVA: 0x000047D3 File Offset: 0x000029D3
			public bool boolValue
			{
				get
				{
					return this._intValue != 0;
				}
			}

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x0600006F RID: 111 RVA: 0x000047DE File Offset: 0x000029DE
			public float floatValue
			{
				get
				{
					return this._floatValue;
				}
			}

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x06000070 RID: 112 RVA: 0x000047E6 File Offset: 0x000029E6
			public string stringValue
			{
				get
				{
					return this._stringValue;
				}
			}

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x06000071 RID: 113 RVA: 0x000047EE File Offset: 0x000029EE
			public UnityEngine.Object objectValue
			{
				get
				{
					return this._objectValue;
				}
			}

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x06000072 RID: 114 RVA: 0x000047F6 File Offset: 0x000029F6
			public AnimationCurve animationCurveValue
			{
				get
				{
					return this._animationCurveValue;
				}
			}

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x06000073 RID: 115 RVA: 0x000047FE File Offset: 0x000029FE
			public Vector3 vector3Value
			{
				get
				{
					return this._vector3Value;
				}
			}

			// Token: 0x06000074 RID: 116 RVA: 0x00004808 File Offset: 0x00002A08
			public bool MatchesFieldInfo(FieldInfo fieldInfo)
			{
				if (fieldInfo == null)
				{
					return false;
				}
				Type fieldType = fieldInfo.FieldType;
				switch (this._valueType)
				{
				case EntityStateManager.StateInfo.Field.ValueType.Invalid:
					return false;
				case EntityStateManager.StateInfo.Field.ValueType.Int:
					return fieldType.IsAssignableFrom(typeof(int));
				case EntityStateManager.StateInfo.Field.ValueType.Float:
					return fieldType.IsAssignableFrom(typeof(float));
				case EntityStateManager.StateInfo.Field.ValueType.String:
					return fieldType.IsAssignableFrom(typeof(string));
				case EntityStateManager.StateInfo.Field.ValueType.Object:
					return this._objectValue == null || fieldType.IsAssignableFrom(this._objectValue.GetType());
				case EntityStateManager.StateInfo.Field.ValueType.Bool:
					return fieldType.IsAssignableFrom(typeof(bool));
				case EntityStateManager.StateInfo.Field.ValueType.AnimationCurve:
					return fieldType.IsAssignableFrom(typeof(AnimationCurve));
				case EntityStateManager.StateInfo.Field.ValueType.Vector3:
					return fieldType.IsAssignableFrom(typeof(Vector3));
				default:
					return false;
				}
			}

			// Token: 0x17000017 RID: 23
			// (get) Token: 0x06000075 RID: 117 RVA: 0x000048E4 File Offset: 0x00002AE4
			public object valueAsSystemObject
			{
				get
				{
					switch (this._valueType)
					{
					case EntityStateManager.StateInfo.Field.ValueType.Invalid:
						return null;
					case EntityStateManager.StateInfo.Field.ValueType.Int:
						return this.intValue;
					case EntityStateManager.StateInfo.Field.ValueType.Float:
						return this.floatValue;
					case EntityStateManager.StateInfo.Field.ValueType.String:
						return this.stringValue;
					case EntityStateManager.StateInfo.Field.ValueType.Object:
						if (!this.objectValue)
						{
							return null;
						}
						return this.objectValue;
					case EntityStateManager.StateInfo.Field.ValueType.Bool:
						return this.boolValue;
					case EntityStateManager.StateInfo.Field.ValueType.AnimationCurve:
						return this.animationCurveValue;
					case EntityStateManager.StateInfo.Field.ValueType.Vector3:
						return this.vector3Value;
					default:
						return null;
					}
				}
			}

			// Token: 0x06000076 RID: 118 RVA: 0x00004977 File Offset: 0x00002B77
			public void Apply(FieldInfo fieldInfo, object instance)
			{
				fieldInfo.SetValue(instance, this.valueAsSystemObject);
			}

			// Token: 0x06000077 RID: 119 RVA: 0x00004986 File Offset: 0x00002B86
			public void ResetValues()
			{
				this._intValue = 0;
				this._floatValue = 0f;
				this._stringValue = null;
				this._objectValue = null;
				this._animationCurveValue = null;
				this._vector3Value = Vector3.zero;
			}

			// Token: 0x06000078 RID: 120 RVA: 0x000049BA File Offset: 0x00002BBA
			public void SetValue(int value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.Int;
				this._intValue = value;
			}

			// Token: 0x06000079 RID: 121 RVA: 0x000049D0 File Offset: 0x00002BD0
			public void SetValue(float value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.Float;
				this._floatValue = value;
			}

			// Token: 0x0600007A RID: 122 RVA: 0x000049E6 File Offset: 0x00002BE6
			public void SetValue(string value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.String;
				this._stringValue = value;
			}

			// Token: 0x0600007B RID: 123 RVA: 0x000049FC File Offset: 0x00002BFC
			public void SetValue(UnityEngine.Object value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.Object;
				this._objectValue = value;
			}

			// Token: 0x0600007C RID: 124 RVA: 0x00004A12 File Offset: 0x00002C12
			public void SetValue(bool value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.Bool;
				this._intValue = (value ? 1 : 0);
			}

			// Token: 0x0600007D RID: 125 RVA: 0x00004A2E File Offset: 0x00002C2E
			public void SetValue(AnimationCurve value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.AnimationCurve;
				this._animationCurveValue = value;
			}

			// Token: 0x0600007E RID: 126 RVA: 0x00004A44 File Offset: 0x00002C44
			public void SetValue(Vector3 value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.Vector3;
				this._vector3Value = value;
			}

			// Token: 0x0600007F RID: 127 RVA: 0x00004A5A File Offset: 0x00002C5A
			public string GetFieldName()
			{
				return this._fieldName;
			}

			// Token: 0x06000080 RID: 128 RVA: 0x00004A62 File Offset: 0x00002C62
			public FieldInfo GetFieldInfo()
			{
				EntityStateManager.StateInfo stateInfo = this.owner;
				if (stateInfo == null)
				{
					return null;
				}
				Type stateType = stateInfo.serializedType.stateType;
				if (stateType == null)
				{
					return null;
				}
				return stateType.GetField(this._fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			}

			// Token: 0x04000082 RID: 130
			[NonSerialized]
			public EntityStateManager.StateInfo owner;

			// Token: 0x04000083 RID: 131
			[SerializeField]
			private string _fieldName;

			// Token: 0x04000084 RID: 132
			[SerializeField]
			private EntityStateManager.StateInfo.Field.ValueType _valueType;

			// Token: 0x04000085 RID: 133
			[SerializeField]
			private int _intValue;

			// Token: 0x04000086 RID: 134
			[SerializeField]
			private float _floatValue;

			// Token: 0x04000087 RID: 135
			[SerializeField]
			private string _stringValue;

			// Token: 0x04000088 RID: 136
			[SerializeField]
			private UnityEngine.Object _objectValue;

			// Token: 0x04000089 RID: 137
			[SerializeField]
			private AnimationCurve _animationCurveValue;

			// Token: 0x0400008A RID: 138
			[SerializeField]
			private Vector3 _vector3Value;

			// Token: 0x02000016 RID: 22
			public enum ValueType
			{
				// Token: 0x0400008C RID: 140
				Invalid,
				// Token: 0x0400008D RID: 141
				Int,
				// Token: 0x0400008E RID: 142
				Float,
				// Token: 0x0400008F RID: 143
				String,
				// Token: 0x04000090 RID: 144
				Object,
				// Token: 0x04000091 RID: 145
				Bool,
				// Token: 0x04000092 RID: 146
				AnimationCurve,
				// Token: 0x04000093 RID: 147
				Vector3
			}
		}

		// Token: 0x02000017 RID: 23
		private struct FieldValuePair
		{
			// Token: 0x04000094 RID: 148
			public FieldInfo fieldInfo;

			// Token: 0x04000095 RID: 149
			public object value;
		}
	}
}
