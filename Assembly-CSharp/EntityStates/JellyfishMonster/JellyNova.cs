using System;
using RoR2;
using UnityEngine;

namespace EntityStates.JellyfishMonster
{
	// Token: 0x02000133 RID: 307
	internal class JellyNova : BaseState
	{
		// Token: 0x060005E8 RID: 1512 RVA: 0x0001B220 File Offset: 0x00019420
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

		// Token: 0x060005E9 RID: 1513 RVA: 0x0001B350 File Offset: 0x00019550
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

		// Token: 0x060005EA RID: 1514 RVA: 0x0001B39F File Offset: 0x0001959F
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

		// Token: 0x060005EB RID: 1515 RVA: 0x0001B3E0 File Offset: 0x000195E0
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
				EffectManager.instance.SpawnEffect(JellyNova.novaEffectPrefab, new EffectData
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
				radius = JellyNova.novaRadius
			}.Fire();
			if (base.healthComponent)
			{
				base.healthComponent.Suicide(null);
			}
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x0000BB2B File Offset: 0x00009D2B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x040006D6 RID: 1750
		public static float baseDuration = 3f;

		// Token: 0x040006D7 RID: 1751
		public static GameObject chargingEffectPrefab;

		// Token: 0x040006D8 RID: 1752
		public static GameObject novaEffectPrefab;

		// Token: 0x040006D9 RID: 1753
		public static string chargingSoundString;

		// Token: 0x040006DA RID: 1754
		public static string novaSoundString;

		// Token: 0x040006DB RID: 1755
		public static float novaDamageCoefficient;

		// Token: 0x040006DC RID: 1756
		public static float novaRadius;

		// Token: 0x040006DD RID: 1757
		public static float novaForce;

		// Token: 0x040006DE RID: 1758
		private bool hasExploded;

		// Token: 0x040006DF RID: 1759
		private float duration;

		// Token: 0x040006E0 RID: 1760
		private float stopwatch;

		// Token: 0x040006E1 RID: 1761
		private GameObject chargeEffect;

		// Token: 0x040006E2 RID: 1762
		private PrintController printController;

		// Token: 0x040006E3 RID: 1763
		private uint soundID;
	}
}
