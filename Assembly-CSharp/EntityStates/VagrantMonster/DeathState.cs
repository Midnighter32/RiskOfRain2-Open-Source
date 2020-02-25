using System;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x02000801 RID: 2049
	public class DeathState : BaseState
	{
		// Token: 0x06002E9A RID: 11930 RVA: 0x000C6294 File Offset: 0x000C4494
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(DeathState.deathString, base.gameObject);
			if (base.modelLocator)
			{
				if (base.modelLocator.modelBaseTransform)
				{
					EntityState.Destroy(base.modelLocator.modelBaseTransform.gameObject);
				}
				if (base.modelLocator.modelTransform)
				{
					EntityState.Destroy(base.modelLocator.modelTransform.gameObject);
				}
			}
			if (base.isAuthority && DeathState.initialExplosion)
			{
				EffectManager.SimpleImpactEffect(DeathState.initialExplosion, base.transform.position, Vector3.up, true);
			}
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x06002E9B RID: 11931 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002BB9 RID: 11193
		public static GameObject initialExplosion;

		// Token: 0x04002BBA RID: 11194
		public static string deathString;
	}
}
