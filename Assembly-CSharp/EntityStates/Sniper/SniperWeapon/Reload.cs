using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Sniper.SniperWeapon
{
	// Token: 0x020000F6 RID: 246
	public class Reload : BaseState
	{
		// Token: 0x060004B5 RID: 1205 RVA: 0x00013AD0 File Offset: 0x00011CD0
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
			if (base.hasAuthority && this.scopeStateMachine)
			{
				this.scopeStateMachine.SetNextState(new LockSkill());
			}
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x00013B90 File Offset: 0x00011D90
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

		// Token: 0x060004B7 RID: 1207 RVA: 0x00013C1A File Offset: 0x00011E1A
		public override void OnExit()
		{
			if (base.hasAuthority && this.scopeStateMachine)
			{
				this.scopeStateMachine.SetNextStateToMain();
			}
			base.OnExit();
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000478 RID: 1144
		public static float baseDuration = 1f;

		// Token: 0x04000479 RID: 1145
		public static float reloadTimeFraction = 0.75f;

		// Token: 0x0400047A RID: 1146
		public static string soundString = "";

		// Token: 0x0400047B RID: 1147
		private float duration;

		// Token: 0x0400047C RID: 1148
		private float reloadTime;

		// Token: 0x0400047D RID: 1149
		private Animator modelAnimator;

		// Token: 0x0400047E RID: 1150
		private bool reloaded;

		// Token: 0x0400047F RID: 1151
		private EntityStateMachine scopeStateMachine;
	}
}
