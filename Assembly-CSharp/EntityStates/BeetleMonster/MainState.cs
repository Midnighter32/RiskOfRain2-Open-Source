using System;
using UnityEngine;

namespace EntityStates.BeetleMonster
{
	// Token: 0x020008E6 RID: 2278
	public class MainState : EntityState
	{
		// Token: 0x060032F9 RID: 13049 RVA: 0x000DD0A1 File Offset: 0x000DB2A1
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				this.modelAnimator.CrossFadeInFixedTime("Walk", 0.1f);
			}
		}

		// Token: 0x060032FA RID: 13050 RVA: 0x000DD0D7 File Offset: 0x000DB2D7
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.modelAnimator)
			{
				this.modelAnimator.SetFloat("walkToRunBlend", 1f);
			}
		}

		// Token: 0x0400325A RID: 12890
		private Animator modelAnimator;
	}
}
