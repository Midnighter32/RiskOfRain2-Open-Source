using System;
using RoR2;
using UnityEngine;

namespace EntityStates.JellyfishMonster
{
	// Token: 0x02000808 RID: 2056
	public class DeathState : BaseState
	{
		// Token: 0x06002EBC RID: 11964 RVA: 0x000C6E10 File Offset: 0x000C5010
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

		// Token: 0x06002EBD RID: 11965 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002BF8 RID: 11256
		public static GameObject initialExplosion;

		// Token: 0x04002BF9 RID: 11257
		public static string deathString;
	}
}
