using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000823 RID: 2083
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06002F35 RID: 12085 RVA: 0x000C9910 File Offset: 0x000C7B10
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
				EffectManager.SimpleMuzzleFlash(DeathState.initialEffect, base.gameObject, "Base", false);
			}
		}

		// Token: 0x06002F36 RID: 12086 RVA: 0x000C9990 File Offset: 0x000C7B90
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.animator)
			{
				this.stopwatch += Time.fixedDeltaTime;
				if (!this.hasPlayedDeathEffect && this.animator.GetFloat("DeathEffect") > 0.5f)
				{
					this.hasPlayedDeathEffect = true;
					EffectManager.SimpleMuzzleFlash(DeathState.deathEffect, base.gameObject, "Center", false);
				}
				if (this.stopwatch >= DeathState.duration)
				{
					EntityState.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04002CB1 RID: 11441
		public static GameObject initialEffect;

		// Token: 0x04002CB2 RID: 11442
		public static GameObject deathEffect;

		// Token: 0x04002CB3 RID: 11443
		private static float duration = 1.333f;

		// Token: 0x04002CB4 RID: 11444
		private float stopwatch;

		// Token: 0x04002CB5 RID: 11445
		private Animator animator;

		// Token: 0x04002CB6 RID: 11446
		private bool hasPlayedDeathEffect;
	}
}
