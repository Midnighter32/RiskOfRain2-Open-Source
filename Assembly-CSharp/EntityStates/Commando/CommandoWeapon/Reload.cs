using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008C7 RID: 2247
	public class Reload : BaseState
	{
		// Token: 0x06003263 RID: 12899 RVA: 0x000D9FCC File Offset: 0x000D81CC
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

		// Token: 0x06003264 RID: 12900 RVA: 0x000DA038 File Offset: 0x000D8238
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

		// Token: 0x06003265 RID: 12901 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400315A RID: 12634
		public static float baseDuration = 1f;

		// Token: 0x0400315B RID: 12635
		public static float reloadTimeFraction = 0.75f;

		// Token: 0x0400315C RID: 12636
		public static string soundString = "";

		// Token: 0x0400315D RID: 12637
		private float duration;

		// Token: 0x0400315E RID: 12638
		private float reloadTime;

		// Token: 0x0400315F RID: 12639
		private Animator modelAnimator;

		// Token: 0x04003160 RID: 12640
		private bool reloaded;
	}
}
