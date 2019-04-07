using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000147 RID: 327
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06000648 RID: 1608 RVA: 0x0001D58C File Offset: 0x0001B78C
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			if (base.characterMotor)
			{
				base.characterMotor.enabled = false;
			}
			if (base.modelLocator && base.modelLocator.modelTransform.GetComponent<ChildLocator>() && DeathState.initialEffect)
			{
				EffectManager.instance.SimpleMuzzleFlash(DeathState.initialEffect, base.gameObject, "Base", false);
			}
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0001D610 File Offset: 0x0001B810
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (!this.hasPlayedDeathEffect && this.animator.GetFloat("DeathEffect") > 0.5f)
			{
				this.hasPlayedDeathEffect = true;
				EffectManager.instance.SimpleMuzzleFlash(DeathState.deathEffect, base.gameObject, "Center", false);
			}
			if (this.stopwatch >= DeathState.duration)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x04000771 RID: 1905
		public static GameObject initialEffect;

		// Token: 0x04000772 RID: 1906
		public static GameObject deathEffect;

		// Token: 0x04000773 RID: 1907
		private static float duration = 1.333f;

		// Token: 0x04000774 RID: 1908
		private float stopwatch;

		// Token: 0x04000775 RID: 1909
		private Animator animator;

		// Token: 0x04000776 RID: 1910
		private bool hasPlayedDeathEffect;
	}
}
