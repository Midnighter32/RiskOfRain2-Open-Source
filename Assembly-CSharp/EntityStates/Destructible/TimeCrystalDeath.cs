using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Destructible
{
	// Token: 0x020008A5 RID: 2213
	public class TimeCrystalDeath : BaseState
	{
		// Token: 0x060031A0 RID: 12704 RVA: 0x000D5BC3 File Offset: 0x000D3DC3
		public override void OnEnter()
		{
			base.OnEnter();
			this.Explode();
		}

		// Token: 0x060031A1 RID: 12705 RVA: 0x000D5BD1 File Offset: 0x000D3DD1
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
		}

		// Token: 0x060031A2 RID: 12706 RVA: 0x000D5BEC File Offset: 0x000D3DEC
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
			if (TimeCrystalDeath.explosionEffectPrefab && NetworkServer.active)
			{
				EffectManager.SpawnEffect(TimeCrystalDeath.explosionEffectPrefab, new EffectData
				{
					origin = base.transform.position,
					scale = TimeCrystalDeath.explosionRadius,
					rotation = Quaternion.identity
				}, true);
			}
			new BlastAttack
			{
				attacker = base.gameObject,
				damageColorIndex = DamageColorIndex.Item,
				baseDamage = this.damageStat * TimeCrystalDeath.explosionDamageCoefficient * Run.instance.teamlessDamageCoefficient,
				radius = TimeCrystalDeath.explosionRadius,
				falloffModel = BlastAttack.FalloffModel.None,
				procCoefficient = TimeCrystalDeath.explosionProcCoefficient,
				teamIndex = TeamIndex.Neutral,
				position = base.transform.position,
				baseForce = TimeCrystalDeath.explosionForce,
				bonusForce = TimeCrystalDeath.explosionForce * 0.5f * Vector3.up
			}.Fire();
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x060031A3 RID: 12707 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400301A RID: 12314
		public static GameObject explosionEffectPrefab;

		// Token: 0x0400301B RID: 12315
		public static float explosionRadius;

		// Token: 0x0400301C RID: 12316
		public static float explosionDamageCoefficient;

		// Token: 0x0400301D RID: 12317
		public static float explosionProcCoefficient;

		// Token: 0x0400301E RID: 12318
		public static float explosionForce;

		// Token: 0x0400301F RID: 12319
		private float stopwatch;
	}
}
