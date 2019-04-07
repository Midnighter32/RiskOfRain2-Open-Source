using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntityStates;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000018 RID: 24
[CreateAssetMenu]
public class EntityStateManager : ScriptableObject, ISerializationCallbackReceiver
{
	// Token: 0x0600007F RID: 127 RVA: 0x0000450C File Offset: 0x0000270C
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

	// Token: 0x06000080 RID: 128 RVA: 0x00004588 File Offset: 0x00002788
	private void ApplyStatic()
	{
		foreach (EntityStateManager.StateInfo stateInfo in this.stateInfoList)
		{
			stateInfo.ApplyStatic();
		}
	}

	// Token: 0x06000081 RID: 129 RVA: 0x000045D8 File Offset: 0x000027D8
	public void Initialize()
	{
		this.ApplyStatic();
		this.GenerateInstanceFieldInitializers();
	}

	// Token: 0x06000082 RID: 130 RVA: 0x000045E8 File Offset: 0x000027E8
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		foreach (EntityStateManager.StateInfo stateInfo2 in this.stateInfoList)
		{
			stateInfo2.RefreshStateType();
		}
		this.stateInfoList.RemoveAll((EntityStateManager.StateInfo stateInfo) => !stateInfo.IsValid());
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00004664 File Offset: 0x00002864
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		foreach (EntityStateManager.StateInfo stateInfo in this.stateInfoList)
		{
			stateInfo.RefreshStateType();
		}
	}

	// Token: 0x06000084 RID: 132 RVA: 0x000046B4 File Offset: 0x000028B4
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

	// Token: 0x06000085 RID: 133 RVA: 0x00004738 File Offset: 0x00002938
	public static void InitializeStateFields(EntityState entityState)
	{
		Action<EntityState> action;
		EntityStateManager.instanceFieldInitializers.TryGetValue(entityState.GetType(), out action);
		if (action != null)
		{
			action(entityState);
		}
	}

	// Token: 0x0400007E RID: 126
	[SerializeField]
	private List<EntityStateManager.StateInfo> stateInfoList = new List<EntityStateManager.StateInfo>();

	// Token: 0x0400007F RID: 127
	private static readonly Dictionary<Type, Action<EntityState>> instanceFieldInitializers = new Dictionary<Type, Action<EntityState>>();

	// Token: 0x02000019 RID: 25
	[Serializable]
	public class GameObjectField
	{
		// Token: 0x04000080 RID: 128
		public string key;

		// Token: 0x04000081 RID: 129
		public GameObject value;
	}

	// Token: 0x0200001A RID: 26
	[Serializable]
	private class StateInfo
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00004781 File Offset: 0x00002981
		private static bool FieldHasSerializeAttribute(FieldInfo fieldInfo)
		{
			return fieldInfo.GetCustomAttributes(typeof(SerializeField), true).Length != 0;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004798 File Offset: 0x00002998
		public void SetStateType(Type stateType)
		{
			this.serializedType.stateType = stateType;
			stateType = this.serializedType.stateType;
			if (stateType == null)
			{
				return;
			}
			IEnumerable<FieldInfo> first = stateType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Where(new Func<FieldInfo, bool>(EntityStateManager.StateInfo.FieldHasSerializeAttribute));
			IEnumerable<FieldInfo> second = from fieldInfo in stateType.GetFields(BindingFlags.Static | BindingFlags.Public)
			where fieldInfo.DeclaringType == stateType
			select fieldInfo;
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

		// Token: 0x0600008B RID: 139 RVA: 0x00004958 File Offset: 0x00002B58
		public void RefreshStateType()
		{
			this.SetStateType(this.serializedType.stateType);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000496C File Offset: 0x00002B6C
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

		// Token: 0x0600008D RID: 141 RVA: 0x000049F8 File Offset: 0x00002BF8
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

		// Token: 0x0600008E RID: 142 RVA: 0x00004AB8 File Offset: 0x00002CB8
		public EntityStateManager.StateInfo.Field FindField(string fieldName)
		{
			return this.stateFieldList.Find((EntityStateManager.StateInfo.Field value) => value.GetFieldName() == fieldName);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004AE9 File Offset: 0x00002CE9
		public bool IsValid()
		{
			return this.serializedType.stateType != null;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004AFC File Offset: 0x00002CFC
		public IList<EntityStateManager.StateInfo.Field> GetFields()
		{
			return this.stateFieldList.AsReadOnly();
		}

		// Token: 0x04000082 RID: 130
		public SerializableEntityStateType serializedType;

		// Token: 0x04000083 RID: 131
		[SerializeField]
		[FormerlySerializedAs("stateStaticFieldList")]
		private List<EntityStateManager.StateInfo.Field> stateFieldList = new List<EntityStateManager.StateInfo.Field>();

		// Token: 0x04000084 RID: 132
		private const BindingFlags defaultInstanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		// Token: 0x04000085 RID: 133
		private const BindingFlags defaultStaticBindingFlags = BindingFlags.Static | BindingFlags.Public;

		// Token: 0x0200001B RID: 27
		[Serializable]
		public class Field
		{
			// Token: 0x06000092 RID: 146 RVA: 0x00004B1C File Offset: 0x00002D1C
			public Field(string fieldName)
			{
				this._fieldName = fieldName;
			}

			// Token: 0x06000093 RID: 147 RVA: 0x00004B2C File Offset: 0x00002D2C
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
				if (this._valueType != valueType)
				{
					this.ResetValues();
					this._valueType = valueType;
				}
			}

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x06000094 RID: 148 RVA: 0x00004BE6 File Offset: 0x00002DE6
			public EntityStateManager.StateInfo.Field.ValueType valueType
			{
				get
				{
					return this._valueType;
				}
			}

			// Token: 0x17000010 RID: 16
			// (get) Token: 0x06000095 RID: 149 RVA: 0x00004BEE File Offset: 0x00002DEE
			public int intValue
			{
				get
				{
					return this._intValue;
				}
			}

			// Token: 0x17000011 RID: 17
			// (get) Token: 0x06000096 RID: 150 RVA: 0x00004BF6 File Offset: 0x00002DF6
			public bool boolValue
			{
				get
				{
					return this._intValue != 0;
				}
			}

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x06000097 RID: 151 RVA: 0x00004C01 File Offset: 0x00002E01
			public float floatValue
			{
				get
				{
					return this._floatValue;
				}
			}

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x06000098 RID: 152 RVA: 0x00004C09 File Offset: 0x00002E09
			public string stringValue
			{
				get
				{
					return this._stringValue;
				}
			}

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x06000099 RID: 153 RVA: 0x00004C11 File Offset: 0x00002E11
			public UnityEngine.Object objectValue
			{
				get
				{
					return this._objectValue;
				}
			}

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x0600009A RID: 154 RVA: 0x00004C19 File Offset: 0x00002E19
			public AnimationCurve animationCurveValue
			{
				get
				{
					return this._animationCurveValue;
				}
			}

			// Token: 0x0600009B RID: 155 RVA: 0x00004C24 File Offset: 0x00002E24
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
				default:
					return false;
				}
			}

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x0600009C RID: 156 RVA: 0x00004CE8 File Offset: 0x00002EE8
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
					default:
						return null;
					}
				}
			}

			// Token: 0x0600009D RID: 157 RVA: 0x00004D6B File Offset: 0x00002F6B
			public void Apply(FieldInfo fieldInfo, object instance)
			{
				fieldInfo.SetValue(instance, this.valueAsSystemObject);
			}

			// Token: 0x0600009E RID: 158 RVA: 0x00004D7A File Offset: 0x00002F7A
			public void ResetValues()
			{
				this._intValue = 0;
				this._floatValue = 0f;
				this._stringValue = null;
				this._objectValue = null;
				this._animationCurveValue = null;
			}

			// Token: 0x0600009F RID: 159 RVA: 0x00004DA3 File Offset: 0x00002FA3
			public void SetValue(int value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.Int;
				this._intValue = value;
			}

			// Token: 0x060000A0 RID: 160 RVA: 0x00004DB9 File Offset: 0x00002FB9
			public void SetValue(float value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.Float;
				this._floatValue = value;
			}

			// Token: 0x060000A1 RID: 161 RVA: 0x00004DCF File Offset: 0x00002FCF
			public void SetValue(string value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.String;
				this._stringValue = value;
			}

			// Token: 0x060000A2 RID: 162 RVA: 0x00004DE5 File Offset: 0x00002FE5
			public void SetValue(UnityEngine.Object value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.Object;
				this._objectValue = value;
			}

			// Token: 0x060000A3 RID: 163 RVA: 0x00004DFB File Offset: 0x00002FFB
			public void SetValue(bool value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.Bool;
				this._intValue = (value ? 1 : 0);
			}

			// Token: 0x060000A4 RID: 164 RVA: 0x00004E17 File Offset: 0x00003017
			public void SetValue(AnimationCurve value)
			{
				this.ResetValues();
				this._valueType = EntityStateManager.StateInfo.Field.ValueType.AnimationCurve;
				this._animationCurveValue = value;
			}

			// Token: 0x060000A5 RID: 165 RVA: 0x00004E2D File Offset: 0x0000302D
			public string GetFieldName()
			{
				return this._fieldName;
			}

			// Token: 0x060000A6 RID: 166 RVA: 0x00004E35 File Offset: 0x00003035
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

			// Token: 0x04000086 RID: 134
			[NonSerialized]
			public EntityStateManager.StateInfo owner;

			// Token: 0x04000087 RID: 135
			[SerializeField]
			private string _fieldName;

			// Token: 0x04000088 RID: 136
			[SerializeField]
			private EntityStateManager.StateInfo.Field.ValueType _valueType;

			// Token: 0x04000089 RID: 137
			[SerializeField]
			private int _intValue;

			// Token: 0x0400008A RID: 138
			[SerializeField]
			private float _floatValue;

			// Token: 0x0400008B RID: 139
			[SerializeField]
			private string _stringValue;

			// Token: 0x0400008C RID: 140
			[SerializeField]
			private UnityEngine.Object _objectValue;

			// Token: 0x0400008D RID: 141
			[SerializeField]
			private AnimationCurve _animationCurveValue;

			// Token: 0x0200001C RID: 28
			public enum ValueType
			{
				// Token: 0x0400008F RID: 143
				Invalid,
				// Token: 0x04000090 RID: 144
				Int,
				// Token: 0x04000091 RID: 145
				Float,
				// Token: 0x04000092 RID: 146
				String,
				// Token: 0x04000093 RID: 147
				Object,
				// Token: 0x04000094 RID: 148
				Bool,
				// Token: 0x04000095 RID: 149
				AnimationCurve
			}
		}

		// Token: 0x0200001D RID: 29
		private struct FieldValuePair
		{
			// Token: 0x04000096 RID: 150
			public FieldInfo fieldInfo;

			// Token: 0x04000097 RID: 151
			public object value;
		}
	}
}
