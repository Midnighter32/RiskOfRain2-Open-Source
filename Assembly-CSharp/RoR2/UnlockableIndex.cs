using System;

namespace RoR2
{
	// Token: 0x020004C7 RID: 1223
	[Serializable]
	public struct UnlockableIndex : IEquatable<UnlockableIndex>, IComparable<UnlockableIndex>
	{
		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06001B7D RID: 7037 RVA: 0x0008045F File Offset: 0x0007E65F
		public int value
		{
			get
			{
				return (int)(this.internalValue - 1u);
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06001B7E RID: 7038 RVA: 0x00080469 File Offset: 0x0007E669
		public bool isValid
		{
			get
			{
				return this.internalValue > 0u;
			}
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x00080474 File Offset: 0x0007E674
		public UnlockableIndex(int value)
		{
			this.internalValue = (uint)(value + 1);
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x0008047F File Offset: 0x0007E67F
		public override bool Equals(object obj)
		{
			return obj is UnlockableIndex && this.Equals((UnlockableIndex)obj);
		}

		// Token: 0x06001B81 RID: 7041 RVA: 0x00080497 File Offset: 0x0007E697
		public bool Equals(UnlockableIndex other)
		{
			return this.internalValue.Equals(other.internalValue);
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x000804AA File Offset: 0x0007E6AA
		public int CompareTo(UnlockableIndex other)
		{
			return this.internalValue.CompareTo(other.internalValue);
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x000804BD File Offset: 0x0007E6BD
		public override int GetHashCode()
		{
			return this.internalValue.GetHashCode();
		}

		// Token: 0x06001B84 RID: 7044 RVA: 0x000804CA File Offset: 0x0007E6CA
		public static bool operator ==(UnlockableIndex a, UnlockableIndex b)
		{
			return a.internalValue == b.internalValue;
		}

		// Token: 0x06001B85 RID: 7045 RVA: 0x000804DA File Offset: 0x0007E6DA
		public static bool operator !=(UnlockableIndex a, UnlockableIndex b)
		{
			return !(a == b);
		}

		// Token: 0x04001E0E RID: 7694
		public uint internalValue;
	}
}
