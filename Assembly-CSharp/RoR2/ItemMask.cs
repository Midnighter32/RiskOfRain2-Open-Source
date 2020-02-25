using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003AD RID: 941
	[Serializable]
	public struct ItemMask
	{
		// Token: 0x060016C8 RID: 5832 RVA: 0x00061BF7 File Offset: 0x0005FDF7
		public bool HasItem(ItemIndex itemIndex)
		{
			if (!ItemCatalog.IsIndexValid(itemIndex))
			{
				return false;
			}
			if (itemIndex < ItemIndex.FireRing)
			{
				return (this.a & 1UL << (int)itemIndex) > 0UL;
			}
			return (this.b & 1UL << itemIndex - ItemIndex.FireRing) > 0UL;
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x00061C31 File Offset: 0x0005FE31
		public void AddItem(ItemIndex itemIndex)
		{
			if (!ItemCatalog.IsIndexValid(itemIndex))
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

		// Token: 0x060016CA RID: 5834 RVA: 0x00061C6E File Offset: 0x0005FE6E
		public void RemoveItem(ItemIndex itemIndex)
		{
			if (!ItemCatalog.IsIndexValid(itemIndex))
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

		// Token: 0x060016CB RID: 5835 RVA: 0x00061CB0 File Offset: 0x0005FEB0
		public static ItemMask operator &(ItemMask mask1, ItemMask mask2)
		{
			return new ItemMask
			{
				a = (mask1.a & mask2.a),
				b = (mask1.b & mask2.b)
			};
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x00061CF0 File Offset: 0x0005FEF0
		static ItemMask()
		{
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				ItemMask.all.AddItem(itemIndex);
				itemIndex++;
			}
		}

		// Token: 0x040015B3 RID: 5555
		[SerializeField]
		public ulong a;

		// Token: 0x040015B4 RID: 5556
		[SerializeField]
		public ulong b;

		// Token: 0x040015B5 RID: 5557
		public static readonly ItemMask none;

		// Token: 0x040015B6 RID: 5558
		public static readonly ItemMask all = default(ItemMask);
	}
}
