using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000714 RID: 1812
	public class HurtStateFlyer : BaseState
	{
		// Token: 0x06002A49 RID: 10825 RVA: 0x000B2154 File Offset: 0x000B0354
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
				this.duration = modelAnimator.GetNextAnimatorStateInfo(layerIndex).length;
			}
		}

		// Token: 0x06002A4A RID: 10826 RVA: 0x000B21FD File Offset: 0x000B03FD
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

		// Token: 0x04002625 RID: 9765
		private float stopwatch;

		// Token: 0x04002626 RID: 9766
		private float duration = 0.35f;
	}
}
