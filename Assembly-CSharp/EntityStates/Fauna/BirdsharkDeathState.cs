using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Fauna
{
	// Token: 0x0200017E RID: 382
	public class BirdsharkDeathState : BaseState
	{
		// Token: 0x06000759 RID: 1881 RVA: 0x00023F88 File Offset: 0x00022188
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(BirdsharkDeathState.deathString, base.gameObject);
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
			if (BirdsharkDeathState.initialExplosion && NetworkServer.active)
			{
				EffectManager.instance.SimpleImpactEffect(BirdsharkDeathState.initialExplosion, base.transform.position, Vector3.up, true);
			}
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000954 RID: 2388
		public static GameObject initialExplosion;

		// Token: 0x04000955 RID: 2389
		public static string deathString;
	}
}
