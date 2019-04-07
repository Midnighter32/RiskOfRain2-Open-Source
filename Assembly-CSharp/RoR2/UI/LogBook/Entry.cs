using System;
using UnityEngine;

namespace RoR2.UI.LogBook
{
	// Token: 0x0200066E RID: 1646
	public class Entry
	{
		// Token: 0x060024B6 RID: 9398 RVA: 0x000ABE60 File Offset: 0x000AA060
		public static GameObject GetModelForPickup(Entry entry)
		{
			return ((PickupIndex)entry.extraData).GetPickupDisplayPrefab();
		}

		// Token: 0x040027BB RID: 10171
		public string nameToken;

		// Token: 0x040027BC RID: 10172
		public string categoryTypeToken;

		// Token: 0x040027BD RID: 10173
		public GameObject pagePrefab;

		// Token: 0x040027BE RID: 10174
		public Texture iconTexture;

		// Token: 0x040027BF RID: 10175
		public Texture bgTexture;

		// Token: 0x040027C0 RID: 10176
		public Color color;

		// Token: 0x040027C1 RID: 10177
		public GameObject modelPrefab;

		// Token: 0x040027C2 RID: 10178
		public object extraData;

		// Token: 0x040027C3 RID: 10179
		public bool isWIP;

		// Token: 0x040027C4 RID: 10180
		public Func<UserProfile, Entry, EntryStatus> getStatus = (UserProfile userProfile, Entry entry) => EntryStatus.Unimplemented;

		// Token: 0x040027C5 RID: 10181
		public Func<UserProfile, Entry, EntryStatus, TooltipContent> getTooltipContent = (UserProfile userProfile, Entry entry, EntryStatus status) => default(TooltipContent);

		// Token: 0x040027C6 RID: 10182
		public UnlockableDef associatedUnlockable;

		// Token: 0x040027C7 RID: 10183
		public Action<PageBuilder> addEntries;

		// Token: 0x040027C8 RID: 10184
		public ViewablesCatalog.Node viewableNode;
	}
}
