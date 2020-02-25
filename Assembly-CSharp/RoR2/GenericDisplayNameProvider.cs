using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200020A RID: 522
	public class GenericDisplayNameProvider : MonoBehaviour, IDisplayNameProvider
	{
		// Token: 0x06000B29 RID: 2857 RVA: 0x000316F2 File Offset: 0x0002F8F2
		public string GetDisplayName()
		{
			return Language.GetString(this.displayToken);
		}

		// Token: 0x04000B91 RID: 2961
		public string displayToken;
	}
}
