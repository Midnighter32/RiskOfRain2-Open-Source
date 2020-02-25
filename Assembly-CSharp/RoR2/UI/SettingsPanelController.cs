using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x02000629 RID: 1577
	public class SettingsPanelController : MonoBehaviour
	{
		// Token: 0x06002543 RID: 9539 RVA: 0x000A23DF File Offset: 0x000A05DF
		private void Start()
		{
			this.settingsControllers = base.GetComponentsInChildren<BaseSettingsControl>();
		}

		// Token: 0x06002544 RID: 9540 RVA: 0x000A23F0 File Offset: 0x000A05F0
		private void Update()
		{
			bool interactable = false;
			for (int i = 0; i < this.settingsControllers.Length; i++)
			{
				if (this.settingsControllers[i].hasBeenChanged)
				{
					interactable = true;
				}
			}
			this.revertButton.interactable = interactable;
		}

		// Token: 0x06002545 RID: 9541 RVA: 0x000A2430 File Offset: 0x000A0630
		public void RevertChanges()
		{
			if (base.isActiveAndEnabled)
			{
				for (int i = 0; i < this.settingsControllers.Length; i++)
				{
					this.settingsControllers[i].Revert();
				}
			}
		}

		// Token: 0x040022F8 RID: 8952
		[FormerlySerializedAs("carouselControllers")]
		private BaseSettingsControl[] settingsControllers;

		// Token: 0x040022F9 RID: 8953
		public MPButton revertButton;
	}
}
