using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x020004A2 RID: 1186
	public struct StatField
	{
		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06001CB3 RID: 7347 RVA: 0x0007AD88 File Offset: 0x00078F88
		public string name
		{
			get
			{
				return this.statDef.name;
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06001CB4 RID: 7348 RVA: 0x0007AD95 File Offset: 0x00078F95
		public StatRecordType recordType
		{
			get
			{
				return this.statDef.recordType;
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06001CB5 RID: 7349 RVA: 0x0007ADA2 File Offset: 0x00078FA2
		public StatDataType dataType
		{
			get
			{
				return this.statDef.dataType;
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06001CB6 RID: 7350 RVA: 0x0007ADAF File Offset: 0x00078FAF
		// (set) Token: 0x06001CB7 RID: 7351 RVA: 0x0007ADBC File Offset: 0x00078FBC
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

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06001CB8 RID: 7352 RVA: 0x0007ADCA File Offset: 0x00078FCA
		// (set) Token: 0x06001CB9 RID: 7353 RVA: 0x0007ADD7 File Offset: 0x00078FD7
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

		// Token: 0x06001CBA RID: 7354 RVA: 0x0007ADE8 File Offset: 0x00078FE8
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

		// Token: 0x06001CBB RID: 7355 RVA: 0x0007AE2C File Offset: 0x0007902C
		public string ToLocalNumeric()
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				return TextSerialization.ToStringNumeric(this.value.ulongValue);
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			return TextSerialization.ToStringNumeric(this.value.doubleValue);
		}

		// Token: 0x06001CBC RID: 7356 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		public ulong CalculatePointValue()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001CBD RID: 7357 RVA: 0x0007AE70 File Offset: 0x00079070
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

		// Token: 0x06001CBE RID: 7358 RVA: 0x0007AF60 File Offset: 0x00079160
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

		// Token: 0x06001CBF RID: 7359 RVA: 0x0007AF9C File Offset: 0x0007919C
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

		// Token: 0x06001CC0 RID: 7360 RVA: 0x0007AFD8 File Offset: 0x000791D8
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

		// Token: 0x06001CC1 RID: 7361 RVA: 0x0007B014 File Offset: 0x00079214
		private void EnforceDataType(StatDataType otherDataType)
		{
			if (this.dataType != otherDataType)
			{
				throw new InvalidOperationException(string.Format("Expected data type {0}, got data type {1}.", this.dataType, otherDataType));
			}
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x0007B040 File Offset: 0x00079240
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

		// Token: 0x06001CC3 RID: 7363 RVA: 0x0007B098 File Offset: 0x00079298
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

		// Token: 0x06001CC4 RID: 7364 RVA: 0x0007B0F0 File Offset: 0x000792F0
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

		// Token: 0x06001CC5 RID: 7365 RVA: 0x0007B13E File Offset: 0x0007933E
		public ulong GetULongValue()
		{
			this.EnforceDataType(StatDataType.ULong);
			return this.ulongValue;
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x0007B14D File Offset: 0x0007934D
		public double GetDoubleValue()
		{
			this.EnforceDataType(StatDataType.Double);
			return this.doubleValue;
		}

		// Token: 0x06001CC7 RID: 7367 RVA: 0x0007B15C File Offset: 0x0007935C
		public bool IsDefault()
		{
			StatDataType dataType = this.dataType;
			if (dataType != StatDataType.ULong)
			{
				return dataType != StatDataType.Double || this.doubleValue == 0.0;
			}
			return this.ulongValue == 0UL;
		}

		// Token: 0x06001CC8 RID: 7368 RVA: 0x0007B198 File Offset: 0x00079398
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

		// Token: 0x06001CC9 RID: 7369 RVA: 0x0007B1D4 File Offset: 0x000793D4
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

		// Token: 0x040019F3 RID: 6643
		public StatDef statDef;

		// Token: 0x040019F4 RID: 6644
		private StatField.ValueUnion value;

		// Token: 0x020004A3 RID: 1187
		[StructLayout(LayoutKind.Explicit)]
		private struct ValueUnion
		{
			// Token: 0x06001CCA RID: 7370 RVA: 0x0007B20D File Offset: 0x0007940D
			public static explicit operator ulong(StatField.ValueUnion v)
			{
				return v.ulongValue;
			}

			// Token: 0x06001CCB RID: 7371 RVA: 0x0007B215 File Offset: 0x00079415
			public static explicit operator double(StatField.ValueUnion v)
			{
				return v.doubleValue;
			}

			// Token: 0x06001CCC RID: 7372 RVA: 0x0007B21D File Offset: 0x0007941D
			public static implicit operator StatField.ValueUnion(ulong ulongValue)
			{
				return new StatField.ValueUnion(ulongValue);
			}

			// Token: 0x06001CCD RID: 7373 RVA: 0x0007B225 File Offset: 0x00079425
			public static implicit operator StatField.ValueUnion(double doubleValue)
			{
				return new StatField.ValueUnion(doubleValue);
			}

			// Token: 0x06001CCE RID: 7374 RVA: 0x0007B22D File Offset: 0x0007942D
			private ValueUnion(ulong ulongValue)
			{
				this = default(StatField.ValueUnion);
				this.ulongValue = ulongValue;
			}

			// Token: 0x06001CCF RID: 7375 RVA: 0x0007B23D File Offset: 0x0007943D
			private ValueUnion(double doubleValue)
			{
				this = default(StatField.ValueUnion);
				this.doubleValue = doubleValue;
			}

			// Token: 0x040019F5 RID: 6645
			[FieldOffset(0)]
			public readonly ulong ulongValue;

			// Token: 0x040019F6 RID: 6646
			[FieldOffset(0)]
			public readonly double doubleValue;
		}
	}
}
