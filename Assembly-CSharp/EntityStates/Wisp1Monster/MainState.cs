using System;
using UnityEngine;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x02000726 RID: 1830
	public class MainState : EntityState
	{
		// Token: 0x06002A95 RID: 10901 RVA: 0x000B33B4 File Offset: 0x000B15B4
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
		}

		// Token: 0x06002A96 RID: 10902 RVA: 0x000B33C8 File Offset: 0x000B15C8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.characterMotor;
			if (this.modelAnimator)
			{
				this.modelAnimator.SetFloat("walkToRunBlend", 1f);
			}
		}

		// Token: 0x04002679 RID: 9849
		private Animator modelAnimator;
	}
}
