using System;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x0200012A RID: 298
	internal class ChargeTrackingBomb : BaseState
	{
		// Token: 0x060005BD RID: 1469 RVA: 0x0001A3E0 File Offset: 0x000185E0
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

		// Token: 0x060005BE RID: 1470 RVA: 0x0001A4CD File Offset: 0x000186CD
		public override void OnExit()
		{
			base.OnExit();
			AkSoundEngine.StopPlayingID(this.soundID);
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x0001A4F8 File Offset: 0x000186F8
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

		// Token: 0x060005C0 RID: 1472 RVA: 0x0000BB2B File Offset: 0x00009D2B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x0400068E RID: 1678
		public static float baseDuration = 3f;

		// Token: 0x0400068F RID: 1679
		public static GameObject chargingEffectPrefab;

		// Token: 0x04000690 RID: 1680
		public static string chargingSoundString;

		// Token: 0x04000691 RID: 1681
		private float duration;

		// Token: 0x04000692 RID: 1682
		private float stopwatch;

		// Token: 0x04000693 RID: 1683
		private GameObject chargeEffectInstance;

		// Token: 0x04000694 RID: 1684
		private uint soundID;
	}
}
