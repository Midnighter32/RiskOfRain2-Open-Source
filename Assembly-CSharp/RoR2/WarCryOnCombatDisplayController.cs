using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200041E RID: 1054
	public class WarCryOnCombatDisplayController : MonoBehaviour
	{
		// Token: 0x0600176C RID: 5996 RVA: 0x0006F1B4 File Offset: 0x0006D3B4
		public void Start()
		{
			CharacterModel component = base.transform.root.gameObject.GetComponent<CharacterModel>();
			if (component)
			{
				this.body = component.body;
			}
			this.UpdateReadyIndicator();
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x0006F1F1 File Offset: 0x0006D3F1
		public void FixedUpdate()
		{
			this.UpdateReadyIndicator();
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x0006F1FC File Offset: 0x0006D3FC
		private void UpdateReadyIndicator()
		{
			bool active = this.body && this.body.warCryReady;
			this.readyIndicator.SetActive(active);
		}

		// Token: 0x04001A9D RID: 6813
		private CharacterBody body;

		// Token: 0x04001A9E RID: 6814
		[Tooltip("The child gameobject to enable when the warcry is ready.")]
		public GameObject readyIndicator;
	}
}
