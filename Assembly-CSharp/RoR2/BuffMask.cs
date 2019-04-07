using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000208 RID: 520
	[Serializable]
	public struct BuffMask : IEquatable<BuffMask>
	{
		// Token: 0x06000A1D RID: 2589 RVA: 0x0003269E File Offset: 0x0003089E
		private BuffMask(uint mask)
		{
			this.mask = mask;
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x000326A7 File Offset: 0x000308A7
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BuffMask GetBuffAdded(BuffIndex buffIndex)
		{
			return new BuffMask(this.mask | 1u << (int)buffIndex);
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x000326BB File Offset: 0x000308BB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BuffMask GetBuffRemoved(BuffIndex buffIndex)
		{
			return new BuffMask(this.mask & ~(1u << (int)buffIndex));
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x000326D0 File Offset: 0x000308D0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasBuff(BuffIndex buffIndex)
		{
			return (this.mask & 1u << (int)buffIndex) > 0u;
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x0000AE8B File Offset: 0x0000908B
		private static bool StaticCheck()
		{
			return true;
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x000326E2 File Offset: 0x000308E2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(BuffMask other)
		{
			return this.mask == other.mask;
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x000326F2 File Offset: 0x000308F2
		public override bool Equals(object obj)
		{
			return obj != null && obj is BuffMask && this.Equals((BuffMask)obj);
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x0003270F File Offset: 0x0003090F
		public override int GetHashCode()
		{
			return (int)this.mask;
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x000326E2 File Offset: 0x000308E2
		public static bool operator ==(BuffMask a, BuffMask b)
		{
			return a.mask == b.mask;
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00032717 File Offset: 0x00030917
		public static bool operator !=(BuffMask a, BuffMask b)
		{
			return a.mask != b.mask;
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x0003272A File Offset: 0x0003092A
		public static void WriteBuffMask(NetworkWriter writer, BuffMask buffMask)
		{
			writer.WritePackedUInt32(buffMask.mask);
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x00032738 File Offset: 0x00030938
		public static BuffMask ReadBuffMask(NetworkReader reader)
		{
			return new BuffMask(reader.ReadPackedUInt32());
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00032748 File Offset: 0x00030948
		static BuffMask()
		{
			for (BuffIndex buffIndex = BuffIndex.Slow50; buffIndex < BuffIndex.Count; buffIndex++)
			{
				if (BuffCatalog.GetBuffDef(buffIndex).isElite)
				{
					BuffMask.eliteMask |= 1u << (int)buffIndex;
				}
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000A2A RID: 2602 RVA: 0x00032780 File Offset: 0x00030980
		public bool containsEliteBuff
		{
			get
			{
				return (this.mask & BuffMask.eliteMask) > 0u;
			}
		}

		// Token: 0x04000D8D RID: 3469
		[SerializeField]
		public readonly uint mask;

		// Token: 0x04000D8E RID: 3470
		private static readonly uint eliteMask;
	}
}
