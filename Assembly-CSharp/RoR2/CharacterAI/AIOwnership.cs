using System;
using UnityEngine;

namespace RoR2.CharacterAI
{
	// Token: 0x02000597 RID: 1431
	[RequireComponent(typeof(BaseAI))]
	public class AIOwnership : MonoBehaviour
	{
		// Token: 0x06002036 RID: 8246 RVA: 0x0009740E File Offset: 0x0009560E
		private void Awake()
		{
			this.baseAI = base.GetComponent<BaseAI>();
		}

		// Token: 0x06002037 RID: 8247 RVA: 0x0009741C File Offset: 0x0009561C
		private void FixedUpdate()
		{
			if (this.ownerMaster)
			{
				this.baseAI.leader.gameObject = this.ownerMaster.GetBodyObject();
			}
		}

		// Token: 0x0400226C RID: 8812
		public CharacterMaster ownerMaster;

		// Token: 0x0400226D RID: 8813
		private BaseAI baseAI;
	}
}
