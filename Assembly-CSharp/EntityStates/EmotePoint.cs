using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000702 RID: 1794
	public class EmotePoint : BaseState
	{
		// Token: 0x060029B2 RID: 10674 RVA: 0x000AFA74 File Offset: 0x000ADC74
		public override void OnEnter()
		{
			base.OnEnter();
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Gesture");
				modelAnimator.SetFloat("EmotePoint.playbackRate", 1f);
				modelAnimator.PlayInFixedTime("EmotePoint", layerIndex, 0f);
				modelAnimator.Update(0f);
				modelAnimator.SetFloat("EmotePoint.playbackRate", this.attackSpeedStat);
			}
		}

		// Token: 0x060029B3 RID: 10675 RVA: 0x000AFAE1 File Offset: 0x000ADCE1
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > EmotePoint.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x040025B0 RID: 9648
		public static float duration = 0.5f;
	}
}
