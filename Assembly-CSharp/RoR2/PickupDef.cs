using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003DF RID: 991
	public class PickupDef
	{
		// Token: 0x170002CD RID: 717
		// (get) Token: 0x0600180D RID: 6157 RVA: 0x00068C19 File Offset: 0x00066E19
		// (set) Token: 0x0600180E RID: 6158 RVA: 0x00068C21 File Offset: 0x00066E21
		public PickupIndex pickupIndex { get; set; } = PickupIndex.none;

		// Token: 0x040016BC RID: 5820
		public string internalName;

		// Token: 0x040016BD RID: 5821
		public GameObject displayPrefab;

		// Token: 0x040016BE RID: 5822
		public GameObject dropletDisplayPrefab;

		// Token: 0x040016BF RID: 5823
		public string nameToken = "???";

		// Token: 0x040016C0 RID: 5824
		public Color baseColor;

		// Token: 0x040016C1 RID: 5825
		public Color darkColor;

		// Token: 0x040016C2 RID: 5826
		public ItemIndex itemIndex = ItemIndex.None;

		// Token: 0x040016C3 RID: 5827
		public EquipmentIndex equipmentIndex = EquipmentIndex.None;

		// Token: 0x040016C4 RID: 5828
		public uint coinValue;

		// Token: 0x040016C5 RID: 5829
		public string unlockableName = "";

		// Token: 0x040016C6 RID: 5830
		public string interactContextToken;

		// Token: 0x040016C7 RID: 5831
		public bool isLunar;

		// Token: 0x040016C8 RID: 5832
		public bool isBoss;

		// Token: 0x040016C9 RID: 5833
		public Texture iconTexture;
	}
}
