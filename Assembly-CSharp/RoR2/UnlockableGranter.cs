using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200040E RID: 1038
	public class UnlockableGranter : MonoBehaviour
	{
		// Token: 0x0600172D RID: 5933 RVA: 0x0006E1DC File Offset: 0x0006C3DC
		public void GrantUnlockable(Interactor interactor)
		{
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component)
			{
				Run.instance.GrantUnlockToSinglePlayer(this.unlockableString, component);
			}
		}

		// Token: 0x04001A55 RID: 6741
		public string unlockableString;
	}
}
