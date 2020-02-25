using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200044E RID: 1102
	public class SurvivorDef
	{
		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06001AD1 RID: 6865 RVA: 0x00071B88 File Offset: 0x0006FD88
		// (set) Token: 0x06001AD2 RID: 6866 RVA: 0x00071B90 File Offset: 0x0006FD90
		public SurvivorIndex survivorIndex { get; set; } = SurvivorIndex.None;

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06001AD3 RID: 6867 RVA: 0x00071B9C File Offset: 0x0006FD9C
		public string displayNameToken
		{
			get
			{
				if (this.bodyPrefab)
				{
					IDisplayNameProvider component = this.bodyPrefab.GetComponent<IDisplayNameProvider>();
					if (component != null)
					{
						return component.GetDisplayName();
					}
				}
				return "";
			}
		}

		// Token: 0x04001859 RID: 6233
		public string name;

		// Token: 0x0400185A RID: 6234
		public GameObject bodyPrefab;

		// Token: 0x0400185B RID: 6235
		public GameObject displayPrefab = Resources.Load<GameObject>("Prefabs/NullModel");

		// Token: 0x0400185D RID: 6237
		public string unlockableName = "";

		// Token: 0x0400185E RID: 6238
		public string descriptionToken;

		// Token: 0x0400185F RID: 6239
		public Color primaryColor;
	}
}
