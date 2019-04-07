using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200029E RID: 670
	public class CloverEffect : MonoBehaviour
	{
		// Token: 0x06000DAE RID: 3502 RVA: 0x000432CC File Offset: 0x000414CC
		private void Start()
		{
			CharacterBody body = base.GetComponentInParent<CharacterModel>().body;
			this.characterBody = body.GetComponent<CharacterBody>();
		}

		// Token: 0x06000DAF RID: 3503 RVA: 0x000432F4 File Offset: 0x000414F4
		private void FixedUpdate()
		{
			if (this.characterBody && this.characterBody.wasLucky)
			{
				this.characterBody.wasLucky = false;
				EffectData effectData = new EffectData();
				effectData.origin = base.transform.position;
				effectData.rotation = base.transform.rotation;
				EffectManager.instance.SpawnEffect(this.triggerEffect, effectData, true);
			}
		}

		// Token: 0x040011A1 RID: 4513
		public GameObject triggerEffect;

		// Token: 0x040011A2 RID: 4514
		private CharacterBody characterBody;

		// Token: 0x040011A3 RID: 4515
		private GameObject triggerEffectInstance;

		// Token: 0x040011A4 RID: 4516
		private bool trigger;
	}
}
