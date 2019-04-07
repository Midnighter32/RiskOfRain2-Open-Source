using System;
using UnityEngine;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x020000CA RID: 202
	public class MainState : EntityState
	{
		// Token: 0x060003F0 RID: 1008 RVA: 0x000103D4 File Offset: 0x0000E5D4
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x000103E8 File Offset: 0x0000E5E8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.characterMotor;
			if (this.modelAnimator)
			{
				this.modelAnimator.SetFloat("walkToRunBlend", 1f);
			}
		}

		// Token: 0x040003B5 RID: 949
		private Animator modelAnimator;
	}
}
