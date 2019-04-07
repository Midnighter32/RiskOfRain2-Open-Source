using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001B2 RID: 434
	public class Reload : BaseState
	{
		// Token: 0x06000881 RID: 2177 RVA: 0x0002AA4C File Offset: 0x00028C4C
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
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x0002AAB8 File Offset: 0x00028CB8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.reloaded && base.fixedAge >= this.reloadTime)
			{
				SkillLocator component = base.gameObject.GetComponent<SkillLocator>();
				if (component)
				{
					GenericSkill primary = component.primary;
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

		// Token: 0x06000883 RID: 2179 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000B59 RID: 2905
		public static float baseDuration = 1f;

		// Token: 0x04000B5A RID: 2906
		public static float reloadTimeFraction = 0.75f;

		// Token: 0x04000B5B RID: 2907
		public static string soundString = "";

		// Token: 0x04000B5C RID: 2908
		private float duration;

		// Token: 0x04000B5D RID: 2909
		private float reloadTime;

		// Token: 0x04000B5E RID: 2910
		private Animator modelAnimator;

		// Token: 0x04000B5F RID: 2911
		private bool reloaded;
	}
}
