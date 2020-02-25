using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000311 RID: 785
	[RequireComponent(typeof(Inventory))]
	public class ScavengerItemGranter : MonoBehaviour
	{
		// Token: 0x06001271 RID: 4721 RVA: 0x0004F6DC File Offset: 0x0004D8DC
		private void Start()
		{
			Inventory component = base.GetComponent<Inventory>();
			List<PickupIndex> list = (from v in Run.instance.availableTier1DropList
			where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).DoesNotContainTag(ItemTag.AIBlacklist)
			select v).ToList<PickupIndex>();
			List<PickupIndex> list2 = (from v in Run.instance.availableTier2DropList
			where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).DoesNotContainTag(ItemTag.AIBlacklist)
			select v).ToList<PickupIndex>();
			List<PickupIndex> list3 = (from v in Run.instance.availableTier3DropList
			where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).DoesNotContainTag(ItemTag.AIBlacklist)
			select v).ToList<PickupIndex>();
			List<PickupIndex> availableEquipmentDropList = Run.instance.availableEquipmentDropList;
			this.GrantItems(component, list, this.tier1Types, this.tier1StackSize);
			this.GrantItems(component, list2, this.tier2Types, this.tier2StackSize);
			this.GrantItems(component, list3, this.tier3Types, this.tier3StackSize);
			component.GiveRandomEquipment();
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x0004F7DC File Offset: 0x0004D9DC
		private void GrantItems(Inventory inventory, List<PickupIndex> list, int types, int stackSize)
		{
			for (int i = 0; i < types; i++)
			{
				inventory.GiveItem(list[UnityEngine.Random.Range(0, list.Count)].itemIndex, stackSize);
			}
		}

		// Token: 0x04001163 RID: 4451
		public int tier1Types;

		// Token: 0x04001164 RID: 4452
		public int tier1StackSize;

		// Token: 0x04001165 RID: 4453
		public int tier2Types;

		// Token: 0x04001166 RID: 4454
		public int tier2StackSize;

		// Token: 0x04001167 RID: 4455
		public int tier3Types;

		// Token: 0x04001168 RID: 4456
		public int tier3StackSize;
	}
}
