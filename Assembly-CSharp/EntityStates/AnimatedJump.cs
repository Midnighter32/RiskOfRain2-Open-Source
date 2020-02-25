using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000700 RID: 1792
	public class AnimatedJump : BaseState
	{
		// Token: 0x060029AB RID: 10667 RVA: 0x000AF8C8 File Offset: 0x000ADAC8
		public override void OnEnter()
		{
			base.OnEnter();
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Body");
				modelAnimator.CrossFadeInFixedTime("AnimatedJump", 0.25f);
				modelAnimator.Update(0f);
				this.duration = modelAnimator.GetNextAnimatorStateInfo(layerIndex).length;
				AimAnimator component = modelAnimator.GetComponent<AimAnimator>();
				if (component)
				{
					component.enabled = true;
				}
			}
		}

		// Token: 0x060029AC RID: 10668 RVA: 0x000AF93C File Offset: 0x000ADB3C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration / 2f && base.isAuthority && !this.hasInputJump)
			{
				this.hasInputJump = true;
				base.characterMotor.moveDirection = base.inputBank.moveVector;
				GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1f, 1f);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x040025AD RID: 9645
		private float duration;

		// Token: 0x040025AE RID: 9646
		private bool hasInputJump;
	}
}
