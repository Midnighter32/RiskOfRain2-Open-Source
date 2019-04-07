using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000BA RID: 186
	public class HurtStateFlyer : BaseState
	{
		// Token: 0x060003AB RID: 939 RVA: 0x0000F3A4 File Offset: 0x0000D5A4
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.sfxLocator && base.sfxLocator.deathSound != "")
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

		// Token: 0x060003AC RID: 940 RVA: 0x0000F44D File Offset: 0x0000D64D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0400036B RID: 875
		private float stopwatch;

		// Token: 0x0400036C RID: 876
		private float duration = 0.35f;
	}
}
