using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003FE RID: 1022
	public class TeslaCoilAnimator : MonoBehaviour
	{
		// Token: 0x060016C1 RID: 5825 RVA: 0x0006C6F4 File Offset: 0x0006A8F4
		private void Start()
		{
			CharacterModel componentInParent = base.GetComponentInParent<CharacterModel>();
			if (componentInParent)
			{
				this.characterBody = componentInParent.body;
			}
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x0006C71C File Offset: 0x0006A91C
		private void FixedUpdate()
		{
			if (this.characterBody)
			{
				this.activeEffectParent.SetActive(this.characterBody.HasBuff(BuffIndex.TeslaField));
			}
		}

		// Token: 0x040019EE RID: 6638
		public GameObject activeEffectParent;

		// Token: 0x040019EF RID: 6639
		private CharacterBody characterBody;
	}
}
