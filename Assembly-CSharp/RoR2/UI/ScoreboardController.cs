using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000631 RID: 1585
	public class ScoreboardController : MonoBehaviour
	{
		// Token: 0x06002390 RID: 9104 RVA: 0x000A7620 File Offset: 0x000A5820
		private void Awake()
		{
			this.stripAllocator = new UIElementAllocator<ScoreboardStrip>(this.container, this.stripPrefab);
		}

		// Token: 0x06002391 RID: 9105 RVA: 0x000A7639 File Offset: 0x000A5839
		private void SetStripCount(int newCount)
		{
			this.stripAllocator.AllocateElements(newCount);
		}

		// Token: 0x06002392 RID: 9106 RVA: 0x000A7648 File Offset: 0x000A5848
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

		// Token: 0x06002393 RID: 9107 RVA: 0x000A7697 File Offset: 0x000A5897
		private void PlayerEventToRebuild(PlayerCharacterMasterController playerCharacterMasterController)
		{
			this.Rebuild();
		}

		// Token: 0x06002394 RID: 9108 RVA: 0x000A769F File Offset: 0x000A589F
		private void OnEnable()
		{
			PlayerCharacterMasterController.onPlayerAdded += this.PlayerEventToRebuild;
			PlayerCharacterMasterController.onPlayerRemoved += this.PlayerEventToRebuild;
			this.Rebuild();
		}

		// Token: 0x06002395 RID: 9109 RVA: 0x000A76C9 File Offset: 0x000A58C9
		private void OnDisable()
		{
			PlayerCharacterMasterController.onPlayerRemoved -= this.PlayerEventToRebuild;
			PlayerCharacterMasterController.onPlayerAdded -= this.PlayerEventToRebuild;
		}

		// Token: 0x04002687 RID: 9863
		public GameObject stripPrefab;

		// Token: 0x04002688 RID: 9864
		public RectTransform container;

		// Token: 0x04002689 RID: 9865
		private UIElementAllocator<ScoreboardStrip> stripAllocator;
	}
}
