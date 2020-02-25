using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Destructible
{
	// Token: 0x020008A3 RID: 2211
	public class ExplosivePotDeath : BaseState
	{
		// Token: 0x06003196 RID: 12694 RVA: 0x000D58D1 File Offset: 0x000D3AD1
		public override void OnEnter()
		{
			base.OnEnter();
			if (ExplosivePotDeath.chargePrefab)
			{
				UnityEngine.Object.Instantiate<GameObject>(ExplosivePotDeath.chargePrefab, base.transform);
			}
		}

		// Token: 0x06003197 RID: 12695 RVA: 0x000D58F6 File Offset: 0x000D3AF6
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge >= ExplosivePotDeath.chargeDuration)
			{
				this.Explode();
			}
		}

		// Token: 0x06003198 RID: 12696 RVA: 0x000D5918 File Offset: 0x000D3B18
		private void Explode()
		{
			if (ExplosivePotDeath.explosionEffectPrefab)
			{
				EffectManager.SpawnEffect(ExplosivePotDeath.explosionEffectPrefab, new EffectData
				{
					origin = base.transform.position,
					scale = ExplosivePotDeath.explosionRadius,
					rotation = Quaternion.identity
				}, true);
			}
			new BlastAttack
			{
				attacker = base.gameObject,
				damageColorIndex = DamageColorIndex.Item,
				baseDamage = this.damageStat * ExplosivePotDeath.explosionDamageCoefficient * Run.instance.teamlessDamageCoefficient,
				radius = ExplosivePotDeath.explosionRadius,
				falloffModel = BlastAttack.FalloffModel.None,
				procCoefficient = ExplosivePotDeath.explosionProcCoefficient,
				teamIndex = TeamIndex.Neutral,
				damageType = DamageType.ClayGoo,
				position = base.transform.position,
				baseForce = ExplosivePotDeath.explosionForce
			}.Fire();
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x06003199 RID: 12697 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400300B RID: 12299
		public static GameObject chargePrefab;

		// Token: 0x0400300C RID: 12300
		public static float chargeDuration;

		// Token: 0x0400300D RID: 12301
		public static GameObject explosionEffectPrefab;

		// Token: 0x0400300E RID: 12302
		public static float explosionRadius;

		// Token: 0x0400300F RID: 12303
		public static float explosionDamageCoefficient;

		// Token: 0x04003010 RID: 12304
		public static float explosionProcCoefficient;

		// Token: 0x04003011 RID: 12305
		public static float explosionForce;
	}
}
