using System;
using UnityEngine;

namespace RoR2.UI.LogBook
{
	// Token: 0x02000663 RID: 1635
	public class Entry
	{
		// Token: 0x0600265A RID: 9818 RVA: 0x000A699C File Offset: 0x000A4B9C
		public static GameObject GetModelForPickup(Entry entry)
		{
			return ((PickupIndex)entry.extraData).GetPickupDisplayPrefab();
		}

		// Token: 0x04002422 RID: 9250
		public string nameToken;

		// Token: 0x04002423 RID: 9251
		public string categoryTypeToken;

		// Token: 0x04002424 RID: 9252
		public GameObject pagePrefab;

		// Token: 0x04002425 RID: 9253
		public Texture iconTexture;

		// Token: 0x04002426 RID: 9254
		public Texture bgTexture;

		// Token: 0x04002427 RID: 9255
		public Color color;

		// Token: 0x04002428 RID: 9256
		public GameObject modelPrefab;

		// Token: 0x04002429 RID: 9257
		public object extraData;

		// Token: 0x0400242A RID: 9258
		public bool isWIP;

		// Token: 0x0400242B RID: 9259
		public Func<UserProfile, Entry, EntryStatus> getStatus = (UserProfile userProfile, Entry entry) => EntryStatus.Unimplemented;

		// Token: 0x0400242C RID: 9260
		public Func<UserProfile, Entry, EntryStatus, TooltipContent> getTooltipContent = (UserProfile userProfile, Entry entry, EntryStatus status) => default(TooltipContent);

		// Token: 0x0400242D RID: 9261
		public UnlockableDef associatedUnlockable;

		// Token: 0x0400242E RID: 9262
		public Action<PageBuilder> addEntries;

		// Token: 0x0400242F RID: 9263
		public ViewablesCatalog.Node viewableNode;
	}
}
