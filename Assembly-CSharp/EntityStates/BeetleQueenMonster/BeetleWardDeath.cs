using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020008EA RID: 2282
	public class BeetleWardDeath : BaseState
	{
		// Token: 0x06003305 RID: 13061 RVA: 0x000DD2C0 File Offset: 0x000DB4C0
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
				EffectManager.SimpleImpactEffect(BeetleWardDeath.initialExplosion, base.transform.position, Vector3.up, true);
			}
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x06003306 RID: 13062 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04003262 RID: 12898
		public static GameObject initialExplosion;

		// Token: 0x04003263 RID: 12899
		public static string deathString;
	}
}
