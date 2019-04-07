using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000BF RID: 191
	public class SleepState : EntityState
	{
		// Token: 0x060003BC RID: 956 RVA: 0x0000F5F4 File Offset: 0x0000D7F4
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

		// Token: 0x060003BD RID: 957 RVA: 0x0000F633 File Offset: 0x0000D833
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0000A1ED File Offset: 0x000083ED
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}
	}
}
