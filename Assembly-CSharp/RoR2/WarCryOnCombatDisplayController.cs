using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037A RID: 890
	public class WarCryOnCombatDisplayController : MonoBehaviour
	{
		// Token: 0x060015A4 RID: 5540 RVA: 0x0005C4B0 File Offset: 0x0005A6B0
		public void Start()
		{
			CharacterModel component = base.transform.root.gameObject.GetComponent<CharacterModel>();
			if (component)
			{
				this.body = component.body;
			}
			this.UpdateReadyIndicator();
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x0005C4ED File Offset: 0x0005A6ED
		public void FixedUpdate()
		{
			this.UpdateReadyIndicator();
		}

		// Token: 0x060015A6 RID: 5542 RVA: 0x0005C4F8 File Offset: 0x0005A6F8
		private void UpdateReadyIndicator()
		{
			bool active = this.body && this.body.warCryReady;
			this.readyIndicator.SetActive(active);
		}

		// Token: 0x04001433 RID: 5171
		private CharacterBody body;

		// Token: 0x04001434 RID: 5172
		[Tooltip("The child gameobject to enable when the warcry is ready.")]
		public GameObject readyIndicator;
	}
}
