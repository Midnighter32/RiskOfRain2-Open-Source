using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000B9 RID: 185
	public class HurtState : BaseState
	{
		// Token: 0x060003A8 RID: 936 RVA: 0x0000F2AC File Offset: 0x0000D4AC
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.sfxLocator && base.sfxLocator.barkSound != "")
			{
				Util.PlaySound(base.sfxLocator.barkSound, base.gameObject);
			}
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Body");
				modelAnimator.CrossFadeInFixedTime((UnityEngine.Random.Range(0, 2) == 0) ? "Hurt1" : "Hurt2", 0.1f);
				modelAnimator.Update(0f);
				this.duration = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;
			}
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0000F355 File Offset: 0x0000D555
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04000369 RID: 873
		private float stopwatch;

		// Token: 0x0400036A RID: 874
		private float duration = 0.35f;
	}
}
