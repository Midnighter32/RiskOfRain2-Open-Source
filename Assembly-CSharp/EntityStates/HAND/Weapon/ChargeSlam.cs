using System;
using UnityEngine;

namespace EntityStates.HAND.Weapon
{
	// Token: 0x02000845 RID: 2117
	public class ChargeSlam : BaseState
	{
		// Token: 0x06002FE9 RID: 12265 RVA: 0x000CD5C8 File Offset: 0x000CB7C8
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

		// Token: 0x06002FEA RID: 12266 RVA: 0x000CD63E File Offset: 0x000CB83E
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.characterMotor.isGrounded && base.isAuthority)
			{
				this.outer.SetNextState(new Slam());
				return;
			}
		}

		// Token: 0x06002FEB RID: 12267 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002DBA RID: 11706
		public static float baseDuration = 3.5f;

		// Token: 0x04002DBB RID: 11707
		private float duration;

		// Token: 0x04002DBC RID: 11708
		private Animator modelAnimator;
	}
}
