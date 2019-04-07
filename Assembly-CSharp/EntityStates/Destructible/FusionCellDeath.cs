using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Destructible
{
	// Token: 0x0200019D RID: 413
	public class FusionCellDeath : BaseState
	{
		// Token: 0x060007FF RID: 2047 RVA: 0x00027994 File Offset: 0x00025B94
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

		// Token: 0x06000800 RID: 2048 RVA: 0x000279DB File Offset: 0x00025BDB
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch > FusionCellDeath.chargeDuration)
			{
				this.Explode();
			}
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x00027A08 File Offset: 0x00025C08
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
				EffectManager.instance.SpawnEffect(FusionCellDeath.explosionEffectPrefab, new EffectData
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

		// Token: 0x06000802 RID: 2050 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000A74 RID: 2676
		public static string chargeChildEffectName;

		// Token: 0x04000A75 RID: 2677
		public static float chargeDuration;

		// Token: 0x04000A76 RID: 2678
		public static GameObject explosionEffectPrefab;

		// Token: 0x04000A77 RID: 2679
		public static float explosionRadius;

		// Token: 0x04000A78 RID: 2680
		public static float explosionDamageCoefficient;

		// Token: 0x04000A79 RID: 2681
		public static float explosionProcCoefficient;

		// Token: 0x04000A7A RID: 2682
		public static float explosionForce;

		// Token: 0x04000A7B RID: 2683
		private float stopwatch;
	}
}
