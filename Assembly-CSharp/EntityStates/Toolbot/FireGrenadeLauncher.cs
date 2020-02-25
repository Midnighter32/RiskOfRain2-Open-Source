using System;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x02000763 RID: 1891
	public class FireGrenadeLauncher : GenericProjectileBaseState
	{
		// Token: 0x06002BAC RID: 11180 RVA: 0x000B87DF File Offset: 0x000B69DF
		protected override void PlayAnimation(float duration)
		{
			base.PlayAnimation("Gesture, Additive", "FireGrenadeLauncher", "FireGrenadeLauncher.playbackRate", duration);
		}

		// Token: 0x06002BAD RID: 11181 RVA: 0x0000AC9A File Offset: 0x00008E9A
		protected override Ray ModifyProjectileAimRay(Ray projectileRay)
		{
			return projectileRay;
		}
	}
}
