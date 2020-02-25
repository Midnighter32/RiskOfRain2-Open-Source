using System;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x02000800 RID: 2048
	public class ChargeTrackingBomb : BaseState
	{
		// Token: 0x06002E94 RID: 11924 RVA: 0x000C6124 File Offset: 0x000C4324
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = ChargeTrackingBomb.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			base.PlayCrossfade("Gesture, Override", "ChargeTrackingBomb", "ChargeTrackingBomb.playbackRate", this.duration, 0.3f);
			this.soundID = Util.PlayScaledSound(ChargeTrackingBomb.chargingSoundString, base.gameObject, this.attackSpeedStat);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("TrackingBombMuzzle");
					if (transform && ChargeTrackingBomb.chargingEffectPrefab)
					{
						this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeTrackingBomb.chargingEffectPrefab, transform.position, transform.rotation);
						this.chargeEffectInstance.transform.parent = transform;
						this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.duration;
					}
				}
			}
		}

		// Token: 0x06002E95 RID: 11925 RVA: 0x000C6211 File Offset: 0x000C4411
		public override void OnExit()
		{
			base.OnExit();
			AkSoundEngine.StopPlayingID(this.soundID);
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
		}

		// Token: 0x06002E96 RID: 11926 RVA: 0x000C623C File Offset: 0x000C443C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new FireTrackingBomb());
				return;
			}
		}

		// Token: 0x06002E97 RID: 11927 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04002BB2 RID: 11186
		public static float baseDuration = 3f;

		// Token: 0x04002BB3 RID: 11187
		public static GameObject chargingEffectPrefab;

		// Token: 0x04002BB4 RID: 11188
		public static string chargingSoundString;

		// Token: 0x04002BB5 RID: 11189
		private float duration;

		// Token: 0x04002BB6 RID: 11190
		private float stopwatch;

		// Token: 0x04002BB7 RID: 11191
		private GameObject chargeEffectInstance;

		// Token: 0x04002BB8 RID: 11192
		private uint soundID;
	}
}
