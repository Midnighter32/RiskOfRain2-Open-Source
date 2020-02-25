using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000367 RID: 871
	public class UnlockableGranter : MonoBehaviour
	{
		// Token: 0x06001534 RID: 5428 RVA: 0x0005A5B0 File Offset: 0x000587B0
		public void GrantUnlockable(Interactor interactor)
		{
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component)
			{
				Run.instance.GrantUnlockToSinglePlayer(this.unlockableString, component);
			}
		}

		// Token: 0x040013C6 RID: 5062
		public string unlockableString;
	}
}
