using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020000D1 RID: 209
	[Serializable]
	public struct BuffMask : IEquatable<BuffMask>
	{
		// Token: 0x06000405 RID: 1029 RVA: 0x0000FECF File Offset: 0x0000E0CF
		private BuffMask(ulong mask)
		{
			this.mask = mask;
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0000FED8 File Offset: 0x0000E0D8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BuffMask GetBuffAdded(BuffIndex buffIndex)
		{
			return new BuffMask(this.mask | 1UL << (int)buffIndex);
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0000FEED File Offset: 0x0000E0ED
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BuffMask GetBuffRemoved(BuffIndex buffIndex)
		{
			return new BuffMask(this.mask & ~(1UL << (int)buffIndex));
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0000FF03 File Offset: 0x0000E103
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasBuff(BuffIndex buffIndex)
		{
			return (this.mask & 1UL << (int)buffIndex) > 0UL;
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x0000B933 File Offset: 0x00009B33
		private static bool StaticCheck()
		{
			return true;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x0000FF17 File Offset: 0x0000E117
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(BuffMask other)
		{
			return this.mask == other.mask;
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x0000FF27 File Offset: 0x0000E127
		public override bool Equals(object obj)
		{
			return obj != null && obj is BuffMask && this.Equals((BuffMask)obj);
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0000FF44 File Offset: 0x0000E144
		public override int GetHashCode()
		{
			return (int)this.mask;
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x0000FF17 File Offset: 0x0000E117
		public static bool operator ==(BuffMask a, BuffMask b)
		{
			return a.mask == b.mask;
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x0000FF4D File Offset: 0x0000E14D
		public static bool operator !=(BuffMask a, BuffMask b)
		{
			return a.mask != b.mask;
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x0000FF60 File Offset: 0x0000E160
		public static void WriteBuffMask(NetworkWriter writer, BuffMask buffMask)
		{
			writer.WritePackedUInt64(buffMask.mask);
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x0000FF6E File Offset: 0x0000E16E
		public static BuffMask ReadBuffMask(NetworkReader reader)
		{
			return new BuffMask(reader.ReadPackedUInt64());
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x0000FF7C File Offset: 0x0000E17C
		[SystemInitializer(new Type[]
		{
			typeof(BuffCatalog)
		})]
		private static void Init()
		{
			BuffIndex buffIndex = BuffIndex.Slow50;
			BuffIndex buffCount = (BuffIndex)BuffCatalog.buffCount;
			while (buffIndex < buffCount)
			{
				if (BuffCatalog.GetBuffDef(buffIndex).isElite)
				{
					BuffMask.eliteMask |= 1UL << (int)buffIndex;
				}
				buffIndex++;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x0000FFBA File Offset: 0x0000E1BA
		public bool containsEliteBuff
		{
			get
			{
				return (this.mask & BuffMask.eliteMask) > 0UL;
			}
		}

		// Token: 0x040003DA RID: 986
		[SerializeField]
		public readonly ulong mask;

		// Token: 0x040003DB RID: 987
		private const ulong maskOne = 1UL;

		// Token: 0x040003DC RID: 988
		private static ulong eliteMask;
	}
}
