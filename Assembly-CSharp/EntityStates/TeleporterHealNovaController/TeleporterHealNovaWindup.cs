using System;
using RoR2;
using UnityEngine;

namespace EntityStates.TeleporterHealNovaController
{
	// Token: 0x02000775 RID: 1909
	public class TeleporterHealNovaWindup : BaseState
	{
		// Token: 0x06002BF3 RID: 11251 RVA: 0x000B9B5A File Offset: 0x000B7D5A
		public override void OnEnter()
		{
			base.OnEnter();
			EffectManager.SimpleEffect(TeleporterHealNovaWindup.chargeEffectPrefab, base.transform.position, Quaternion.identity, false);
		}

		// Token: 0x06002BF4 RID: 11252 RVA: 0x000B9B7D File Offset: 0x000B7D7D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && TeleporterHealNovaWindup.duration <= base.fixedAge)
			{
				this.outer.SetNextState(new TeleporterHealNovaPulse());
			}
		}

		// Token: 0x0400280F RID: 10255
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002810 RID: 10256
		public static float duration;
	}
}
