using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Sniper.SniperWeapon
{
	// Token: 0x02000788 RID: 1928
	public class Reload : BaseState
	{
		// Token: 0x06002C45 RID: 11333 RVA: 0x000BAE08 File Offset: 0x000B9008
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Reload.baseDuration / this.attackSpeedStat;
			this.reloadTime = this.duration * Reload.reloadTimeFraction;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayAnimation("Gesture", "PrepBarrage", "PrepBarrage.playbackRate", this.duration);
			}
			if (base.skillLocator)
			{
				GenericSkill secondary = base.skillLocator.secondary;
				if (secondary)
				{
					this.scopeStateMachine = secondary.stateMachine;
				}
			}
			if (base.isAuthority && this.scopeStateMachine)
			{
				this.scopeStateMachine.SetNextState(new LockSkill());
			}
		}

		// Token: 0x06002C46 RID: 11334 RVA: 0x000BAEC8 File Offset: 0x000B90C8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.reloaded && base.fixedAge >= this.reloadTime)
			{
				if (base.skillLocator)
				{
					GenericSkill primary = base.skillLocator.primary;
					if (primary)
					{
						primary.Reset();
						Util.PlaySound(Reload.soundString, base.gameObject);
					}
				}
				this.reloaded = true;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002C47 RID: 11335 RVA: 0x000BAF52 File Offset: 0x000B9152
		public override void OnExit()
		{
			if (base.isAuthority && this.scopeStateMachine)
			{
				this.scopeStateMachine.SetNextStateToMain();
			}
			base.OnExit();
		}

		// Token: 0x06002C48 RID: 11336 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400284B RID: 10315
		public static float baseDuration = 1f;

		// Token: 0x0400284C RID: 10316
		public static float reloadTimeFraction = 0.75f;

		// Token: 0x0400284D RID: 10317
		public static string soundString = "";

		// Token: 0x0400284E RID: 10318
		private float duration;

		// Token: 0x0400284F RID: 10319
		private float reloadTime;

		// Token: 0x04002850 RID: 10320
		private Animator modelAnimator;

		// Token: 0x04002851 RID: 10321
		private bool reloaded;

		// Token: 0x04002852 RID: 10322
		private EntityStateMachine scopeStateMachine;
	}
}
