using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000713 RID: 1811
	public class HurtState : BaseState
	{
		// Token: 0x06002A46 RID: 10822 RVA: 0x000B205C File Offset: 0x000B025C
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
				this.duration = modelAnimator.GetNextAnimatorStateInfo(layerIndex).length;
			}
		}

		// Token: 0x06002A47 RID: 10823 RVA: 0x000B2105 File Offset: 0x000B0305
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04002623 RID: 9763
		private float stopwatch;

		// Token: 0x04002624 RID: 9764
		private float duration = 0.35f;
	}
}
