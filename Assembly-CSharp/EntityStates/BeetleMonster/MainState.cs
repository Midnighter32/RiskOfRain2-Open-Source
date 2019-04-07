using System;
using UnityEngine;

namespace EntityStates.BeetleMonster
{
	// Token: 0x020001CB RID: 459
	public class MainState : EntityState
	{
		// Token: 0x060008F9 RID: 2297 RVA: 0x0002D431 File Offset: 0x0002B631
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				this.modelAnimator.CrossFadeInFixedTime("Walk", 0.1f);
			}
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x0002D467 File Offset: 0x0002B667
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.modelAnimator)
			{
				this.modelAnimator.SetFloat("walkToRunBlend", 1f);
			}
		}

		// Token: 0x04000C32 RID: 3122
		private Animator modelAnimator;
	}
}
