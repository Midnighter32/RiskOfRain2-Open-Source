using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x02000636 RID: 1590
	public class SettingsPanelController : MonoBehaviour
	{
		// Token: 0x060023A9 RID: 9129 RVA: 0x000A7B57 File Offset: 0x000A5D57
		private void Start()
		{
			this.settingsControllers = base.GetComponentsInChildren<BaseSettingsControl>();
		}

		// Token: 0x060023AA RID: 9130 RVA: 0x000A7B68 File Offset: 0x000A5D68
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

		// Token: 0x060023AB RID: 9131 RVA: 0x000A7BA8 File Offset: 0x000A5DA8
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

		// Token: 0x0400269B RID: 9883
		[FormerlySerializedAs("carouselControllers")]
		private BaseSettingsControl[] settingsControllers;

		// Token: 0x0400269C RID: 9884
		public MPButton revertButton;
	}
}
