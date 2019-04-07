using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004B8 RID: 1208
	public class SurvivorDef
	{
		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06001B48 RID: 6984 RVA: 0x0007F854 File Offset: 0x0007DA54
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

		// Token: 0x04001DE2 RID: 7650
		public GameObject bodyPrefab;

		// Token: 0x04001DE3 RID: 7651
		public GameObject displayPrefab = Resources.Load<GameObject>("Prefabs/NullModel");

		// Token: 0x04001DE4 RID: 7652
		public SurvivorIndex survivorIndex;

		// Token: 0x04001DE5 RID: 7653
		public string unlockableName = "";

		// Token: 0x04001DE6 RID: 7654
		public string descriptionToken;

		// Token: 0x04001DE7 RID: 7655
		public Color primaryColor;
	}
}
