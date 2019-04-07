using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Destructible
{
	// Token: 0x0200019E RID: 414
	public class TimeCrystalDeath : BaseState
	{
		// Token: 0x06000804 RID: 2052 RVA: 0x00027B5F File Offset: 0x00025D5F
		public override void OnEnter()
		{
			base.OnEnter();
			this.Explode();
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x00027B6D File Offset: 0x00025D6D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x00027B88 File Offset: 0x00025D88
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
				EffectManager.instance.SpawnEffect(TimeCrystalDeath.explosionEffectPrefab, new EffectData
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

		// Token: 0x06000807 RID: 2055 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000A7C RID: 2684
		public static GameObject explosionEffectPrefab;

		// Token: 0x04000A7D RID: 2685
		public static float explosionRadius;

		// Token: 0x04000A7E RID: 2686
		public static float explosionDamageCoefficient;

		// Token: 0x04000A7F RID: 2687
		public static float explosionProcCoefficient;

		// Token: 0x04000A80 RID: 2688
		public static float explosionForce;

		// Token: 0x04000A81 RID: 2689
		private float stopwatch;
	}
}
