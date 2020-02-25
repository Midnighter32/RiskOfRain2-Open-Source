using System;

namespace RoR2
{
	// Token: 0x0200045F RID: 1119
	[Serializable]
	public struct UnlockableIndex : IEquatable<UnlockableIndex>, IComparable<UnlockableIndex>
	{
		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06001B1B RID: 6939 RVA: 0x00072BA2 File Offset: 0x00070DA2
		public int value
		{
			get
			{
				return (int)(this.internalValue - 1U);
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06001B1C RID: 6940 RVA: 0x00072BAC File Offset: 0x00070DAC
		public bool isValid
		{
			get
			{
				return this.internalValue > 0U;
			}
		}

		// Token: 0x06001B1D RID: 6941 RVA: 0x00072BB7 File Offset: 0x00070DB7
		public UnlockableIndex(int value)
		{
			this.internalValue = (uint)(value + 1);
		}

		// Token: 0x06001B1E RID: 6942 RVA: 0x00072BC2 File Offset: 0x00070DC2
		public override bool Equals(object obj)
		{
			return obj is UnlockableIndex && this.Equals((UnlockableIndex)obj);
		}

		// Token: 0x06001B1F RID: 6943 RVA: 0x00072BDA File Offset: 0x00070DDA
		public bool Equals(UnlockableIndex other)
		{
			return this.internalValue.Equals(other.internalValue);
		}

		// Token: 0x06001B20 RID: 6944 RVA: 0x00072BED File Offset: 0x00070DED
		public int CompareTo(UnlockableIndex other)
		{
			return this.internalValue.CompareTo(other.internalValue);
		}

		// Token: 0x06001B21 RID: 6945 RVA: 0x00072C00 File Offset: 0x00070E00
		public override int GetHashCode()
		{
			return this.internalValue.GetHashCode();
		}

		// Token: 0x06001B22 RID: 6946 RVA: 0x00072C0D File Offset: 0x00070E0D
		public static bool operator ==(UnlockableIndex a, UnlockableIndex b)
		{
			return a.internalValue == b.internalValue;
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x00072C1D File Offset: 0x00070E1D
		public static bool operator !=(UnlockableIndex a, UnlockableIndex b)
		{
			return !(a == b);
		}

		// Token: 0x0400188C RID: 6284
		public uint internalValue;
	}
}
