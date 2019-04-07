using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Destructible
{
	// Token: 0x0200019C RID: 412
	public class ExplosivePotDeath : BaseState
	{
		// Token: 0x060007FA RID: 2042 RVA: 0x00027862 File Offset: 0x00025A62
		public override void OnEnter()
		{
			base.OnEnter();
			if (ExplosivePotDeath.chargePrefab)
			{
				UnityEngine.Object.Instantiate<GameObject>(ExplosivePotDeath.chargePrefab, base.transform);
			}
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00027887 File Offset: 0x00025A87
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge >= ExplosivePotDeath.chargeDuration)
			{
				this.Explode();
			}
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x000278AC File Offset: 0x00025AAC
		private void Explode()
		{
			if (ExplosivePotDeath.explosionEffectPrefab)
			{
				EffectManager.instance.SpawnEffect(ExplosivePotDeath.explosionEffectPrefab, new EffectData
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

		// Token: 0x060007FD RID: 2045 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000A6D RID: 2669
		public static GameObject chargePrefab;

		// Token: 0x04000A6E RID: 2670
		public static float chargeDuration;

		// Token: 0x04000A6F RID: 2671
		public static GameObject explosionEffectPrefab;

		// Token: 0x04000A70 RID: 2672
		public static float explosionRadius;

		// Token: 0x04000A71 RID: 2673
		public static float explosionDamageCoefficient;

		// Token: 0x04000A72 RID: 2674
		public static float explosionProcCoefficient;

		// Token: 0x04000A73 RID: 2675
		public static float explosionForce;
	}
}
