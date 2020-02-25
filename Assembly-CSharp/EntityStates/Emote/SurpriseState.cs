using System;
using UnityEngine;

namespace EntityStates.Emote
{
	// Token: 0x02000784 RID: 1924
	public class SurpriseState : EntityState
	{
		// Token: 0x06002C2F RID: 11311 RVA: 0x000BA788 File Offset: 0x000B8988
		public override void OnEnter()
		{
			base.OnEnter();
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Body");
				modelAnimator.Play("EmoteSurprise", layerIndex, 0f);
				modelAnimator.Update(0f);
				this.duration = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;
			}
		}

		// Token: 0x06002C30 RID: 11312 RVA: 0x000BA7E7 File Offset: 0x000B89E7
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002C31 RID: 11313 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400282E RID: 10286
		private float duration;
	}
}
