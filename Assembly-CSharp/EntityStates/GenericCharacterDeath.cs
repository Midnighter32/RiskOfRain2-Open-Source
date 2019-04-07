using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x020000B4 RID: 180
	public class GenericCharacterDeath : BaseState
	{
		// Token: 0x06000390 RID: 912 RVA: 0x0000E656 File Offset: 0x0000C856
		protected virtual float GetDeathAnimationCrossFadeDuration()
		{
			return 0.1f;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0000E65D File Offset: 0x0000C85D
		public override void OnEnter()
		{
			base.OnEnter();
			this.cullingStopwatch = 0f;
			this.PlayDeathSound();
			this.PlayDeathAnimation(0.1f);
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0000E681 File Offset: 0x0000C881
		protected void PlayDeathSound()
		{
			if (base.sfxLocator && base.sfxLocator.deathSound != "")
			{
				Util.PlaySound(base.sfxLocator.deathSound, base.gameObject);
			}
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0000E6C0 File Offset: 0x0000C8C0
		protected void PlayDeathAnimation(float crossfadeDuration = 0.1f)
		{
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				modelAnimator.CrossFadeInFixedTime("Death", crossfadeDuration);
			}
		}

		// Token: 0x06000394 RID: 916 RVA: 0x0000E6E8 File Offset: 0x0000C8E8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.cullingStopwatch += Time.fixedDeltaTime;
			if (NetworkServer.active && base.characterMotor && base.characterMotor.atRest)
			{
				this.cullingStopwatch += Time.fixedDeltaTime;
				if (this.cullingStopwatch >= 1f && base.characterMotor && base.characterMotor.atRest)
				{
					EntityState.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000355 RID: 853
		private const float bodyPreservationDuration = 1f;

		// Token: 0x04000356 RID: 854
		private float cullingStopwatch;
	}
}
