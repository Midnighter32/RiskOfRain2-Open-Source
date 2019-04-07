using System;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x02000129 RID: 297
	internal class ChargeMegaNova : BaseState
	{
		// Token: 0x060005B7 RID: 1463 RVA: 0x0001A18C File Offset: 0x0001838C
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = ChargeMegaNova.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			base.PlayCrossfade("Gesture, Override", "ChargeMegaNova", "ChargeMegaNova.playbackRate", this.duration, 0.3f);
			this.soundID = Util.PlayScaledSound(ChargeMegaNova.chargingSoundString, base.gameObject, this.attackSpeedStat);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("HullCenter");
					Transform transform2 = component.FindChild("NovaCenter");
					if (transform && ChargeMegaNova.chargingEffectPrefab)
					{
						this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMegaNova.chargingEffectPrefab, transform.position, transform.rotation);
						this.chargeEffectInstance.transform.localScale = new Vector3(ChargeMegaNova.novaRadius, ChargeMegaNova.novaRadius, ChargeMegaNova.novaRadius);
						this.chargeEffectInstance.transform.parent = transform;
						this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.duration;
					}
					if (transform2 && ChargeMegaNova.areaIndicatorPrefab)
					{
						this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMegaNova.areaIndicatorPrefab, transform2.position, transform2.rotation);
						this.areaIndicatorInstance.transform.localScale = new Vector3(ChargeMegaNova.novaRadius * 2f, ChargeMegaNova.novaRadius * 2f, ChargeMegaNova.novaRadius * 2f);
						this.areaIndicatorInstance.transform.parent = transform2;
					}
				}
			}
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x0001A328 File Offset: 0x00018528
		public override void OnExit()
		{
			base.OnExit();
			AkSoundEngine.StopPlayingID(this.soundID);
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
			if (this.areaIndicatorInstance)
			{
				EntityState.Destroy(this.areaIndicatorInstance);
			}
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x0001A378 File Offset: 0x00018578
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				FireMegaNova fireMegaNova = new FireMegaNova();
				fireMegaNova.novaRadius = ChargeMegaNova.novaRadius;
				this.outer.SetNextState(fireMegaNova);
				return;
			}
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x0000BB2B File Offset: 0x00009D2B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04000684 RID: 1668
		public static float baseDuration = 3f;

		// Token: 0x04000685 RID: 1669
		public static GameObject chargingEffectPrefab;

		// Token: 0x04000686 RID: 1670
		public static GameObject areaIndicatorPrefab;

		// Token: 0x04000687 RID: 1671
		public static string chargingSoundString;

		// Token: 0x04000688 RID: 1672
		public static float novaRadius;

		// Token: 0x04000689 RID: 1673
		private float duration;

		// Token: 0x0400068A RID: 1674
		private float stopwatch;

		// Token: 0x0400068B RID: 1675
		private GameObject chargeEffectInstance;

		// Token: 0x0400068C RID: 1676
		private GameObject areaIndicatorInstance;

		// Token: 0x0400068D RID: 1677
		private uint soundID;
	}
}
