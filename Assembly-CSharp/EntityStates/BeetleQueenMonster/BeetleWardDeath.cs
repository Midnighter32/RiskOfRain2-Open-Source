using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020001CF RID: 463
	public class BeetleWardDeath : BaseState
	{
		// Token: 0x06000905 RID: 2309 RVA: 0x0002D650 File Offset: 0x0002B850
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(BeetleWardDeath.deathString, base.gameObject);
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
			if (BeetleWardDeath.initialExplosion && NetworkServer.active)
			{
				EffectManager.instance.SimpleImpactEffect(BeetleWardDeath.initialExplosion, base.transform.position, Vector3.up, true);
			}
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000C3A RID: 3130
		public static GameObject initialExplosion;

		// Token: 0x04000C3B RID: 3131
		public static string deathString;
	}
}
