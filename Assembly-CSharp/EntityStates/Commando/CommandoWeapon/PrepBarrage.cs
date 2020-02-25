using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008C5 RID: 2245
	public class PrepBarrage : BaseState
	{
		// Token: 0x06003258 RID: 12888 RVA: 0x000D9D54 File Offset: 0x000D7F54
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepBarrage.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayAnimation("Gesture", "PrepBarrage", "PrepBarrage.playbackRate", this.duration);
			}
			Util.PlaySound(PrepBarrage.prepBarrageSoundString, base.gameObject);
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
		}

		// Token: 0x06003259 RID: 12889 RVA: 0x000D9DDC File Offset: 0x000D7FDC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireBarrage nextState = new FireBarrage();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x0600325A RID: 12890 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400314E RID: 12622
		public static float baseDuration = 3f;

		// Token: 0x0400314F RID: 12623
		public static string prepBarrageSoundString;

		// Token: 0x04003150 RID: 12624
		private float duration;

		// Token: 0x04003151 RID: 12625
		private Animator modelAnimator;
	}
}
