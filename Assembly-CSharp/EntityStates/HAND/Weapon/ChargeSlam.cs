using System;
using UnityEngine;

namespace EntityStates.HAND.Weapon
{
	// Token: 0x02000163 RID: 355
	public class ChargeSlam : BaseState
	{
		// Token: 0x060006E4 RID: 1764 RVA: 0x00020EB8 File Offset: 0x0001F0B8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeSlam.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayAnimation("Gesture", "ChargeSlam", "ChargeSlam.playbackRate", this.duration);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(4f);
			}
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00020F2E File Offset: 0x0001F12E
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.characterMotor.isGrounded && base.isAuthority)
			{
				this.outer.SetNextState(new Slam());
				return;
			}
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000874 RID: 2164
		public static float baseDuration = 3.5f;

		// Token: 0x04000875 RID: 2165
		private float duration;

		// Token: 0x04000876 RID: 2166
		private Animator modelAnimator;
	}
}
