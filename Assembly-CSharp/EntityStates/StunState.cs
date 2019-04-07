using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000C1 RID: 193
	public class StunState : BaseState
	{
		// Token: 0x060003C5 RID: 965 RVA: 0x0000F7D8 File Offset: 0x0000D9D8
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
				AnimatorStateInfo currentAnimatorStateInfo = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex);
				this.duration = Mathf.Max(this.stunDuration, currentAnimatorStateInfo.length);
				this.stunEffect = UnityEngine.Object.Instantiate<GameObject>(StunState.stunEffectPrefab, base.transform);
				this.stunEffect.GetComponent<ScaleParticleSystemDuration>().newDuration = this.duration;
			}
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0000F8BB File Offset: 0x0000DABB
		public override void OnExit()
		{
			if (this.stunEffect)
			{
				EntityState.Destroy(this.stunEffect);
			}
			base.OnExit();
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0000F8DB File Offset: 0x0000DADB
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04000378 RID: 888
		private float stopwatch;

		// Token: 0x04000379 RID: 889
		private float duration;

		// Token: 0x0400037A RID: 890
		private GameObject stunEffect;

		// Token: 0x0400037B RID: 891
		public float stunDuration = 0.35f;

		// Token: 0x0400037C RID: 892
		public static GameObject stunEffectPrefab;
	}
}
