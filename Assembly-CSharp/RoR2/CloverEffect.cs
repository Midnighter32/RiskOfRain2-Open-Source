using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001AA RID: 426
	public class CloverEffect : MonoBehaviour
	{
		// Token: 0x0600091D RID: 2333 RVA: 0x00027588 File Offset: 0x00025788
		private void Start()
		{
			CharacterBody body = base.GetComponentInParent<CharacterModel>().body;
			this.characterBody = body.GetComponent<CharacterBody>();
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x000275B0 File Offset: 0x000257B0
		private void FixedUpdate()
		{
			if (this.characterBody && this.characterBody.wasLucky)
			{
				this.characterBody.wasLucky = false;
				EffectData effectData = new EffectData();
				effectData.origin = base.transform.position;
				effectData.rotation = base.transform.rotation;
				EffectManager.SpawnEffect(this.triggerEffect, effectData, true);
			}
		}

		// Token: 0x04000973 RID: 2419
		public GameObject triggerEffect;

		// Token: 0x04000974 RID: 2420
		private CharacterBody characterBody;

		// Token: 0x04000975 RID: 2421
		private GameObject triggerEffectInstance;

		// Token: 0x04000976 RID: 2422
		private bool trigger;
	}
}
