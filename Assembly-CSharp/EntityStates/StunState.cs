using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x0200071B RID: 1819
	public class StunState : BaseState
	{
		// Token: 0x06002A63 RID: 10851 RVA: 0x000B2594 File Offset: 0x000B0794
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
				AnimatorStateInfo nextAnimatorStateInfo = modelAnimator.GetNextAnimatorStateInfo(layerIndex);
				this.duration = Mathf.Max(this.stunDuration, nextAnimatorStateInfo.length);
				if (this.stunDuration >= 0f)
				{
					this.stunVfxInstance = UnityEngine.Object.Instantiate<GameObject>(StunState.stunVfxPrefab, base.transform);
					this.stunVfxInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.duration;
				}
			}
		}

		// Token: 0x06002A64 RID: 10852 RVA: 0x000B2684 File Offset: 0x000B0884
		public override void OnExit()
		{
			if (this.stunVfxInstance)
			{
				EntityState.Destroy(this.stunVfxInstance);
			}
			base.OnExit();
		}

		// Token: 0x06002A65 RID: 10853 RVA: 0x000B26A4 File Offset: 0x000B08A4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04002632 RID: 9778
		private float stopwatch;

		// Token: 0x04002633 RID: 9779
		private float duration;

		// Token: 0x04002634 RID: 9780
		private GameObject stunVfxInstance;

		// Token: 0x04002635 RID: 9781
		public float stunDuration = 0.35f;

		// Token: 0x04002636 RID: 9782
		public static GameObject stunVfxPrefab;
	}
}
