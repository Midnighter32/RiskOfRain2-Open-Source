using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Destructible
{
	// Token: 0x020008A4 RID: 2212
	public class FusionCellDeath : BaseState
	{
		// Token: 0x0600319B RID: 12699 RVA: 0x000D59FC File Offset: 0x000D3BFC
		public override void OnEnter()
		{
			base.OnEnter();
			ChildLocator component = base.GetModelTransform().GetComponent<ChildLocator>();
			if (component)
			{
				Transform transform = component.FindChild(FusionCellDeath.chargeChildEffectName);
				if (transform)
				{
					transform.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x0600319C RID: 12700 RVA: 0x000D5A43 File Offset: 0x000D3C43
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch > FusionCellDeath.chargeDuration)
			{
				this.Explode();
			}
		}

		// Token: 0x0600319D RID: 12701 RVA: 0x000D5A70 File Offset: 0x000D3C70
		private void Explode()
		{
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
			if (FusionCellDeath.explosionEffectPrefab && NetworkServer.active)
			{
				EffectManager.SpawnEffect(FusionCellDeath.explosionEffectPrefab, new EffectData
				{
					origin = base.transform.position,
					scale = FusionCellDeath.explosionRadius,
					rotation = Quaternion.identity
				}, true);
			}
			new BlastAttack
			{
				attacker = base.gameObject,
				damageColorIndex = DamageColorIndex.Item,
				baseDamage = this.damageStat * FusionCellDeath.explosionDamageCoefficient * Run.instance.teamlessDamageCoefficient,
				radius = FusionCellDeath.explosionRadius,
				falloffModel = BlastAttack.FalloffModel.None,
				procCoefficient = FusionCellDeath.explosionProcCoefficient,
				teamIndex = TeamIndex.Neutral,
				position = base.transform.position,
				baseForce = FusionCellDeath.explosionForce,
				bonusForce = FusionCellDeath.explosionForce * 0.5f * Vector3.up
			}.Fire();
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x0600319E RID: 12702 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04003012 RID: 12306
		public static string chargeChildEffectName;

		// Token: 0x04003013 RID: 12307
		public static float chargeDuration;

		// Token: 0x04003014 RID: 12308
		public static GameObject explosionEffectPrefab;

		// Token: 0x04003015 RID: 12309
		public static float explosionRadius;

		// Token: 0x04003016 RID: 12310
		public static float explosionDamageCoefficient;

		// Token: 0x04003017 RID: 12311
		public static float explosionProcCoefficient;

		// Token: 0x04003018 RID: 12312
		public static float explosionForce;

		// Token: 0x04003019 RID: 12313
		private float stopwatch;
	}
}
