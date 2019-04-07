using System;
using UnityEngine;

namespace EntityStates.Emote
{
	// Token: 0x020000F3 RID: 243
	public class SurpriseState : EntityState
	{
		// Token: 0x060004A5 RID: 1189 RVA: 0x00013550 File Offset: 0x00011750
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

		// Token: 0x060004A6 RID: 1190 RVA: 0x000135AF File Offset: 0x000117AF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400045C RID: 1116
		private float duration;
	}
}
