using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000215 RID: 533
	[RequireComponent(typeof(Inventory))]
	public class GivePickupsOnStart : MonoBehaviour
	{
		// Token: 0x06000BB9 RID: 3001 RVA: 0x0003314C File Offset: 0x0003134C
		private void Start()
		{
			this.inventory = base.GetComponent<Inventory>();
			this.inventory.SetEquipmentIndex(this.equipmentIndex);
			for (int i = 0; i < this.itemInfos.Length; i++)
			{
				GivePickupsOnStart.ItemInfo itemInfo = this.itemInfos[i];
				this.inventory.GiveItem(itemInfo.itemIndex, itemInfo.count);
			}
		}

		// Token: 0x04000BD3 RID: 3027
		public EquipmentIndex equipmentIndex;

		// Token: 0x04000BD4 RID: 3028
		public GivePickupsOnStart.ItemInfo[] itemInfos;

		// Token: 0x04000BD5 RID: 3029
		private Inventory inventory;

		// Token: 0x02000216 RID: 534
		[Serializable]
		public struct ItemInfo
		{
			// Token: 0x04000BD6 RID: 3030
			public ItemIndex itemIndex;

			// Token: 0x04000BD7 RID: 3031
			public int count;
		}
	}
}
