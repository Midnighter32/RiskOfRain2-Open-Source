using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000AB RID: 171
	public class EmotePoint : BaseState
	{
		// Token: 0x06000338 RID: 824 RVA: 0x0000D53C File Offset: 0x0000B73C
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

		// Token: 0x06000339 RID: 825 RVA: 0x0000D5A9 File Offset: 0x0000B7A9
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > EmotePoint.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0400031F RID: 799
		public static float duration = 0.5f;
	}
}
