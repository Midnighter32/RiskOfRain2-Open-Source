using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x020004FE RID: 1278
	public struct StatField
	{
		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06001CD6 RID: 7382 RVA: 0x00086744 File Offset: 0x00084944
		public string name
		{
			get
			{
				return this.statDef.name;
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06001CD7 RID: 7383 RVA: 0x00086751 File Offset: 0x00084951
		public StatRecordType recordType
		{
			get
			{
				return this.statDef.recordType;
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06001CD8 RID: 7384 RVA: 0x0008675E File Offset: 0x0008495E
		public StatDataType dataType
		{
			get
			{
				return this.statDef.dataType;
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06001CD9 RID: 7385 RVA: 0x0008676B File Offset: 0x0008496B
		// (set) Token: 0x06001CDA RID: 7386 RVA: 0x00086778 File Offset: 0x00084978
		private ulong ulongValue
		{
			get
			{
				return this.value.ulongValue;
			}
			set
			{
				this.value = value;
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06001CDB RID: 7387 RVA: 0x00086786 File Offset: 0x00084986
		// (set) Token: 0x06001CDC RID: 7388 RVA: 0x00086793 File Offset: 0x00084993
		private double doubleValue
		{
			get
			{
				return this.value.doubleValue;
			}
			set
			{
				this.value = value;
			}
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x000867A4 File Offset: 0x000849A4
		public override string ToString()
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				return TextSerialization.ToStringInvariant(this.value.ulongValue);
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			return TextSerialization.ToStringInvariant(this.value.doubleValue);
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x0007E12C File Offset: 0x0007C32C
		public ulong CalculatePointValue()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x000867E8 File Offset: 0x000849E8
		[Pure]
		public static StatField GetDelta(ref StatField newerValue, ref StatField olderValue)
		{
			StatField result = new StatField
			{
				statDef = newerValue.statDef
			};
			StatDataType dataType = newerValue.dataType;
			if (dataType != StatDataType.ULong)
			{
				if (dataType == StatDataType.Double)
				{
					switch (newerValue.recordType)
					{
					case StatRecordType.Sum:
						result.doubleValue = newerValue.doubleValue - olderValue.doubleValue;
						break;
					case StatRecordType.Max:
						result.doubleValue = Math.Max(newerValue.doubleValue, olderValue.doubleValue);
						break;
					case StatRecordType.Newest:
						result.doubleValue = newerValue.doubleValue;
						break;
					}
				}
			}
			else
			{
				switch (newerValue.recordType)
				{
				case StatRecordType.Sum:
					result.ulongValue = newerValue.ulongValue - olderValue.ulongValue;
					break;
				case StatRecordType.Max:
					result.ulongValue = Math.Max(newerValue.ulongValue, olderValue.ulongValue);
					break;
				case StatRecordType.Newest:
					result.ulongValue = newerValue.ulongValue;
					break;
				}
			}
			return result;
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x000868D8 File Offset: 0x00084AD8
		public void PushDelta(ref StatField deltaField)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				this.PushStatValue(deltaField.ulongValue);
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.PushStatValue(deltaField.doubleValue);
		}

		// Token: 0x06001CE1 RID: 7393 RVA: 0x00086914 File Offset: 0x00084B14
		public void Write(NetworkWriter writer)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				writer.WritePackedUInt64(this.ulongValue);
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			writer.Write(this.doubleValue);
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x00086950 File Offset: 0x00084B50
		public void Read(NetworkReader reader)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				this.ulongValue = reader.ReadPackedUInt64();
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.doubleValue = reader.ReadDouble();
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x0008698C File Offset: 0x00084B8C
		private void EnforceDataType(StatDataType otherDataType)
		{
			if (this.dataType != otherDataType)
			{
				throw new InvalidOperationException(string.Format("Expected data type {0}, got data type {1}.", this.dataType, otherDataType));
			}
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x000869B8 File Offset: 0x00084BB8
		public void PushStatValue(ulong incomingValue)
		{
			this.EnforceDataType(StatDataType.ULong);
			switch (this.recordType)
			{
			case StatRecordType.Sum:
				this.ulongValue += incomingValue;
				return;
			case StatRecordType.Max:
				this.ulongValue = Math.Max(incomingValue, this.ulongValue);
				return;
			case StatRecordType.Newest:
				this.ulongValue = incomingValue;
				return;
			default:
				return;
			}
		}

		// Token: 0x06001CE5 RID: 7397 RVA: 0x00086A10 File Offset: 0x00084C10
		public void PushStatValue(double incomingValue)
		{
			this.EnforceDataType(StatDataType.Double);
			switch (this.recordType)
			{
			case StatRecordType.Sum:
				this.doubleValue += incomingValue;
				return;
			case StatRecordType.Max:
				this.doubleValue = Math.Max(incomingValue, this.doubleValue);
				return;
			case StatRecordType.Newest:
				this.doubleValue = incomingValue;
				return;
			default:
				return;
			}
		}

		// Token: 0x06001CE6 RID: 7398 RVA: 0x00086A68 File Offset: 0x00084C68
		public void SetFromString(string valueString)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				ulong ulongValue;
				TextSerialization.TryParseInvariant(valueString, out ulongValue);
				this.value = ulongValue;
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			double doubleValue;
			TextSerialization.TryParseInvariant(valueString, out doubleValue);
			this.value = doubleValue;
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x00086AB6 File Offset: 0x00084CB6
		public ulong GetULongValue()
		{
			this.EnforceDataType(StatDataType.ULong);
			return this.ulongValue;
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x00086AC5 File Offset: 0x00084CC5
		public double GetDoubleValue()
		{
			this.EnforceDataType(StatDataType.Double);
			return this.doubleValue;
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x00086AD4 File Offset: 0x00084CD4
		public bool IsDefault()
		{
			StatDataType dataType = this.dataType;
			if (dataType != StatDataType.ULong)
			{
				return dataType != StatDataType.Double || this.doubleValue == 0.0;
			}
			return this.ulongValue == 0UL;
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x00086B10 File Offset: 0x00084D10
		public void SetDefault()
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				this.ulongValue = 0UL;
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new NotImplementedException();
			}
			this.doubleValue = 0.0;
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x00086B4C File Offset: 0x00084D4C
		public ulong GetPointValue(double pointValue)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				return (ulong)(this.ulongValue * pointValue);
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			return (ulong)(this.doubleValue * pointValue);
		}

		// Token: 0x04001F26 RID: 7974
		public StatDef statDef;

		// Token: 0x04001F27 RID: 7975
		private StatField.ValueUnion value;

		// Token: 0x020004FF RID: 1279
		[StructLayout(LayoutKind.Explicit)]
		private struct ValueUnion
		{
			// Token: 0x06001CEC RID: 7404 RVA: 0x00086B85 File Offset: 0x00084D85
			public static explicit operator ulong(StatField.ValueUnion v)
			{
				return v.ulongValue;
			}

			// Token: 0x06001CED RID: 7405 RVA: 0x00086B8D File Offset: 0x00084D8D
			public static explicit operator double(StatField.ValueUnion v)
			{
				return v.doubleValue;
			}

			// Token: 0x06001CEE RID: 7406 RVA: 0x00086B95 File Offset: 0x00084D95
			public static implicit operator StatField.ValueUnion(ulong ulongValue)
			{
				return new StatField.ValueUnion(ulongValue);
			}

			// Token: 0x06001CEF RID: 7407 RVA: 0x00086B9D File Offset: 0x00084D9D
			public static implicit operator StatField.ValueUnion(double doubleValue)
			{
				return new StatField.ValueUnion(doubleValue);
			}

			// Token: 0x06001CF0 RID: 7408 RVA: 0x00086BA5 File Offset: 0x00084DA5
			private ValueUnion(ulong ulongValue)
			{
				this = default(StatField.ValueUnion);
				this.ulongValue = ulongValue;
			}

			// Token: 0x06001CF1 RID: 7409 RVA: 0x00086BB5 File Offset: 0x00084DB5
			private ValueUnion(double doubleValue)
			{
				this = default(StatField.ValueUnion);
				this.doubleValue = doubleValue;
			}

			// Token: 0x04001F28 RID: 7976
			[FieldOffset(0)]
			public readonly ulong ulongValue;

			// Token: 0x04001F29 RID: 7977
			[FieldOffset(0)]
			public readonly double doubleValue;
		}
	}
}
