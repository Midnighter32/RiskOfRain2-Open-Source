using System;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x020007FF RID: 2047
	public class ChargeMegaNova : BaseState
	{
		// Token: 0x06002E8E RID: 11918 RVA: 0x000C5ED0 File Offset: 0x000C40D0
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

		// Token: 0x06002E8F RID: 11919 RVA: 0x000C606C File Offset: 0x000C426C
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

		// Token: 0x06002E90 RID: 11920 RVA: 0x000C60BC File Offset: 0x000C42BC
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

		// Token: 0x06002E91 RID: 11921 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04002BA8 RID: 11176
		public static float baseDuration = 3f;

		// Token: 0x04002BA9 RID: 11177
		public static GameObject chargingEffectPrefab;

		// Token: 0x04002BAA RID: 11178
		public static GameObject areaIndicatorPrefab;

		// Token: 0x04002BAB RID: 11179
		public static string chargingSoundString;

		// Token: 0x04002BAC RID: 11180
		public static float novaRadius;

		// Token: 0x04002BAD RID: 11181
		private float duration;

		// Token: 0x04002BAE RID: 11182
		private float stopwatch;

		// Token: 0x04002BAF RID: 11183
		private GameObject chargeEffectInstance;

		// Token: 0x04002BB0 RID: 11184
		private GameObject areaIndicatorInstance;

		// Token: 0x04002BB1 RID: 11185
		private uint soundID;
	}
}
