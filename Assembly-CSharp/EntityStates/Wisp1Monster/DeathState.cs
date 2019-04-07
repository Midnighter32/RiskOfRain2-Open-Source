using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x020000C7 RID: 199
	public class DeathState : BaseState
	{
		// Token: 0x060003E1 RID: 993 RVA: 0x00010048 File Offset: 0x0000E248
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

		// Token: 0x060003E2 RID: 994 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040003A1 RID: 929
		public static GameObject initialExplosion;

		// Token: 0x040003A2 RID: 930
		public static string deathString;
	}
}
