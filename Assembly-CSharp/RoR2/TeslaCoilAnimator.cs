using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000356 RID: 854
	public class TeslaCoilAnimator : MonoBehaviour
	{
		// Token: 0x060014C0 RID: 5312 RVA: 0x000589BC File Offset: 0x00056BBC
		private void Start()
		{
			CharacterModel componentInParent = base.GetComponentInParent<CharacterModel>();
			if (componentInParent)
			{
				this.characterBody = componentInParent.body;
			}
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x000589E4 File Offset: 0x00056BE4
		private void FixedUpdate()
		{
			if (this.characterBody)
			{
				this.activeEffectParent.SetActive(this.characterBody.HasBuff(BuffIndex.TeslaField));
			}
		}

		// Token: 0x04001355 RID: 4949
		public GameObject activeEffectParent;

		// Token: 0x04001356 RID: 4950
		private CharacterBody characterBody;
	}
}
