using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000448 RID: 1096
	[Serializable]
	public struct ItemMask
	{
		// Token: 0x0600185D RID: 6237 RVA: 0x00074085 File Offset: 0x00072285
		public bool HasItem(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= ItemIndex.Count)
			{
				return false;
			}
			if (itemIndex < ItemIndex.FireRing)
			{
				return (this.a & 1UL << (int)itemIndex) > 0UL;
			}
			return (this.b & 1UL << itemIndex - ItemIndex.FireRing) > 0UL;
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x000740BF File Offset: 0x000722BF
		public void AddItem(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= ItemIndex.Count)
			{
				return;
			}
			if (itemIndex < ItemIndex.FireRing)
			{
				this.a |= 1UL << (int)itemIndex;
				return;
			}
			this.b |= 1UL << itemIndex - ItemIndex.FireRing;
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x000740FC File Offset: 0x000722FC
		public void RemoveItem(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= ItemIndex.Count)
			{
				return;
			}
			if (itemIndex < ItemIndex.FireRing)
			{
				this.a &= ~(1UL << (int)itemIndex);
				return;
			}
			this.b &= ~(1UL << itemIndex - ItemIndex.FireRing);
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x0007413C File Offset: 0x0007233C
		public static ItemMask operator &(ItemMask mask1, ItemMask mask2)
		{
			return new ItemMask
			{
				a = (mask1.a & mask2.a),
				b = (mask1.b & mask2.b)
			};
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x0007417C File Offset: 0x0007237C
		static ItemMask()
		{
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				ItemMask.all.AddItem(itemIndex);
			}
		}

		// Token: 0x04001BEF RID: 7151
		[SerializeField]
		public ulong a;

		// Token: 0x04001BF0 RID: 7152
		[SerializeField]
		public ulong b;

		// Token: 0x04001BF1 RID: 7153
		public static readonly ItemMask none;

		// Token: 0x04001BF2 RID: 7154
		public static readonly ItemMask all = default(ItemMask);
	}
}
