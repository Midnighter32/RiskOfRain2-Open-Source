using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001B0 RID: 432
	internal class PrepBarrage : BaseState
	{
		// Token: 0x06000876 RID: 2166 RVA: 0x0002A7D4 File Offset: 0x000289D4
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

		// Token: 0x06000877 RID: 2167 RVA: 0x0002A85C File Offset: 0x00028A5C
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

		// Token: 0x06000878 RID: 2168 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000B4D RID: 2893
		public static float baseDuration = 3f;

		// Token: 0x04000B4E RID: 2894
		public static string prepBarrageSoundString;

		// Token: 0x04000B4F RID: 2895
		private float duration;

		// Token: 0x04000B50 RID: 2896
		private Animator modelAnimator;
	}
}
