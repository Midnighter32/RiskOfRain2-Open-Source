using System;
using RoR2;
using UnityEngine;

namespace EntityStates.JellyfishMonster
{
	// Token: 0x02000809 RID: 2057
	public class JellyNova : BaseState
	{
		// Token: 0x06002EBF RID: 11967 RVA: 0x000C6EC8 File Offset: 0x000C50C8
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = JellyNova.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			base.PlayCrossfade("Body", "Nova", "Nova.playbackRate", this.duration, 0.1f);
			this.soundID = Util.PlaySound(JellyNova.chargingSoundString, base.gameObject);
			if (JellyNova.chargingEffectPrefab)
			{
				this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(JellyNova.chargingEffectPrefab, base.transform.position, base.transform.rotation);
				this.chargeEffect.transform.parent = base.transform;
				this.chargeEffect.transform.localScale = new Vector3(JellyNova.novaRadius, JellyNova.novaRadius, JellyNova.novaRadius);
				this.chargeEffect.GetComponent<ScaleParticleSystemDuration>().newDuration = this.duration;
			}
			if (modelTransform)
			{
				this.printController = modelTransform.GetComponent<PrintController>();
				if (this.printController)
				{
					this.printController.enabled = true;
					this.printController.printTime = this.duration;
				}
			}
		}

		// Token: 0x06002EC0 RID: 11968 RVA: 0x000C6FF8 File Offset: 0x000C51F8
		public override void OnExit()
		{
			base.OnExit();
			AkSoundEngine.StopPlayingID(this.soundID);
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
			if (this.printController)
			{
				this.printController.enabled = false;
			}
		}

		// Token: 0x06002EC1 RID: 11969 RVA: 0x000C7047 File Offset: 0x000C5247
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority && !this.hasExploded)
			{
				this.Detonate();
				return;
			}
		}

		// Token: 0x06002EC2 RID: 11970 RVA: 0x000C7088 File Offset: 0x000C5288
		private void Detonate()
		{
			this.hasExploded = true;
			Util.PlaySound(JellyNova.novaSoundString, base.gameObject);
			if (base.modelLocator)
			{
				if (base.modelLocator.modelBaseTransform)
				{
					EntityState.Destroy(base.modelLocator.modelBaseTransform.gameObject);
				}
				if (base.modelLocator.modelTransform)
				{
					EntityState.Destroy(base.modelLocator.modelTransform.gameObject);
				}
			}
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
			if (JellyNova.novaEffectPrefab)
			{
				EffectManager.SpawnEffect(JellyNova.novaEffectPrefab, new EffectData
				{
					origin = base.transform.position,
					scale = JellyNova.novaRadius
				}, true);
			}
			new BlastAttack
			{
				attacker = base.gameObject,
				inflictor = base.gameObject,
				teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
				baseDamage = this.damageStat * JellyNova.novaDamageCoefficient,
				baseForce = JellyNova.novaForce,
				position = base.transform.position,
				radius = JellyNova.novaRadius,
				procCoefficient = 2f
			}.Fire();
			if (base.healthComponent)
			{
				base.healthComponent.Suicide(null, null, DamageType.Generic);
			}
		}

		// Token: 0x06002EC3 RID: 11971 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04002BFA RID: 11258
		public static float baseDuration = 3f;

		// Token: 0x04002BFB RID: 11259
		public static GameObject chargingEffectPrefab;

		// Token: 0x04002BFC RID: 11260
		public static GameObject novaEffectPrefab;

		// Token: 0x04002BFD RID: 11261
		public static string chargingSoundString;

		// Token: 0x04002BFE RID: 11262
		public static string novaSoundString;

		// Token: 0x04002BFF RID: 11263
		public static float novaDamageCoefficient;

		// Token: 0x04002C00 RID: 11264
		public static float novaRadius;

		// Token: 0x04002C01 RID: 11265
		public static float novaForce;

		// Token: 0x04002C02 RID: 11266
		private bool hasExploded;

		// Token: 0x04002C03 RID: 11267
		private float duration;

		// Token: 0x04002C04 RID: 11268
		private float stopwatch;

		// Token: 0x04002C05 RID: 11269
		private GameObject chargeEffect;

		// Token: 0x04002C06 RID: 11270
		private PrintController printController;

		// Token: 0x04002C07 RID: 11271
		private uint soundID;
	}
}
