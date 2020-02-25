using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000719 RID: 1817
	public class SleepState : EntityState
	{
		// Token: 0x06002A5A RID: 10842 RVA: 0x000B2390 File Offset: 0x000B0590
		public override void OnEnter()
		{
			base.OnEnter();
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Body");
				modelAnimator.Play("Sleep", layerIndex, 0f);
			}
		}

		// Token: 0x06002A5B RID: 10843 RVA: 0x000B23CF File Offset: 0x000B05CF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		// Token: 0x06002A5C RID: 10844 RVA: 0x0000AC89 File Offset: 0x00008E89
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}
	}
}
