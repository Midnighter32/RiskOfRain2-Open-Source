using System;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x0200012B RID: 299
	public class DeathState : BaseState
	{
		// Token: 0x060005C3 RID: 1475 RVA: 0x0001A550 File Offset: 0x00018750
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
				EffectManager.instance.SimpleImpactEffect(DeathState.initialExplosion, base.transform.position, Vector3.up, true);
			}
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000695 RID: 1685
		public static GameObject initialExplosion;

		// Token: 0x04000696 RID: 1686
		public static string deathString;
	}
}
