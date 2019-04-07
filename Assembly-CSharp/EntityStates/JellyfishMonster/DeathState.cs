using System;
using RoR2;
using UnityEngine;

namespace EntityStates.JellyfishMonster
{
	// Token: 0x02000132 RID: 306
	public class DeathState : BaseState
	{
		// Token: 0x060005E5 RID: 1509 RVA: 0x0001B168 File Offset: 0x00019368
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
			if (DeathState.initialExplosion)
			{
				UnityEngine.Object.Instantiate<GameObject>(DeathState.initialExplosion, base.transform.position, base.transform.rotation);
			}
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040006D4 RID: 1748
		public static GameObject initialExplosion;

		// Token: 0x040006D5 RID: 1749
		public static string deathString;
	}
}
