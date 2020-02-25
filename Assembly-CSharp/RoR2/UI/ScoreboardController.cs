using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200061F RID: 1567
	public class ScoreboardController : MonoBehaviour
	{
		// Token: 0x0600250D RID: 9485 RVA: 0x000A19B4 File Offset: 0x0009FBB4
		private void Awake()
		{
			this.stripAllocator = new UIElementAllocator<ScoreboardStrip>(this.container, this.stripPrefab);
		}

		// Token: 0x0600250E RID: 9486 RVA: 0x000A19CD File Offset: 0x0009FBCD
		private void SetStripCount(int newCount)
		{
			this.stripAllocator.AllocateElements(newCount);
		}

		// Token: 0x0600250F RID: 9487 RVA: 0x000A19DC File Offset: 0x0009FBDC
		private void Rebuild()
		{
			ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
			int count = instances.Count;
			this.SetStripCount(count);
			for (int i = 0; i < count; i++)
			{
				this.stripAllocator.elements[i].SetMaster(instances[i].master);
			}
		}

		// Token: 0x06002510 RID: 9488 RVA: 0x000A1A2B File Offset: 0x0009FC2B
		private void PlayerEventToRebuild(PlayerCharacterMasterController playerCharacterMasterController)
		{
			this.Rebuild();
		}

		// Token: 0x06002511 RID: 9489 RVA: 0x000A1A33 File Offset: 0x0009FC33
		private void OnEnable()
		{
			PlayerCharacterMasterController.onPlayerAdded += this.PlayerEventToRebuild;
			PlayerCharacterMasterController.onPlayerRemoved += this.PlayerEventToRebuild;
			this.Rebuild();
		}

		// Token: 0x06002512 RID: 9490 RVA: 0x000A1A5D File Offset: 0x0009FC5D
		private void OnDisable()
		{
			PlayerCharacterMasterController.onPlayerRemoved -= this.PlayerEventToRebuild;
			PlayerCharacterMasterController.onPlayerAdded -= this.PlayerEventToRebuild;
		}

		// Token: 0x040022CE RID: 8910
		public GameObject stripPrefab;

		// Token: 0x040022CF RID: 8911
		public RectTransform container;

		// Token: 0x040022D0 RID: 8912
		private UIElementAllocator<ScoreboardStrip> stripAllocator;
	}
}
