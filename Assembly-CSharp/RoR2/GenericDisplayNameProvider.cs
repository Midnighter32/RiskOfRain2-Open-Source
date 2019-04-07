using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002FB RID: 763
	public class GenericDisplayNameProvider : MonoBehaviour, IDisplayNameProvider
	{
		// Token: 0x06000F71 RID: 3953 RVA: 0x0004C296 File Offset: 0x0004A496
		public string GetDisplayName()
		{
			return Language.GetString(this.displayToken);
		}

		// Token: 0x04001391 RID: 5009
		public string displayToken;
	}
}
