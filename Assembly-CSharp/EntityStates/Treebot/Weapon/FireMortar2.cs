using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x02000752 RID: 1874
	public class FireMortar2 : BaseState
	{
		// Token: 0x06002B64 RID: 11108 RVA: 0x000B6D2C File Offset: 0x000B4F2C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireMortar2.baseDuration / this.attackSpeedStat;
			EffectManager.SimpleMuzzleFlash(FireMortar2.muzzleEffect, base.gameObject, FireMortar2.muzzleName, false);
			Util.PlaySound(FireMortar2.fireSound, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "FireBomb", 0.1f);
			if (base.isAuthority)
			{
				this.Fire();
			}
			if (NetworkServer.active && base.healthComponent)
			{
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = base.healthComponent.combinedHealth * FireMortar2.healthCostFraction;
				damageInfo.position = base.characterBody.corePosition;
				damageInfo.force = Vector3.zero;
				damageInfo.damageColorIndex = DamageColorIndex.Default;
				damageInfo.crit = false;
				damageInfo.attacker = null;
				damageInfo.inflictor = null;
				damageInfo.damageType = (DamageType.NonLethal | DamageType.BypassArmor);
				damageInfo.procCoefficient = 0f;
				damageInfo.procChainMask = default(ProcChainMask);
				base.healthComponent.TakeDamage(damageInfo);
			}
		}

		// Token: 0x06002B65 RID: 11109 RVA: 0x000B6E31 File Offset: 0x000B5031
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002B66 RID: 11110 RVA: 0x000B6E5C File Offset: 0x000B505C
		private void Fire()
		{
			RaycastHit raycastHit;
			Vector3 point;
			if (base.inputBank.GetAimRaycast(FireMortar2.maxDistance, out raycastHit))
			{
				point = raycastHit.point;
			}
			else
			{
				point = base.inputBank.GetAimRay().GetPoint(FireMortar2.maxDistance);
			}
			FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
			{
				projectilePrefab = FireMortar2.projectilePrefab,
				position = point,
				rotation = Quaternion.identity,
				owner = base.gameObject,
				damage = FireMortar2.damageCoefficient * this.damageStat,
				force = FireMortar2.force,
				crit = base.RollCrit(),
				damageColorIndex = DamageColorIndex.Default,
				target = null,
				speedOverride = 0f,
				fuseOverride = -1f
			};
			ProjectileManager.instance.FireProjectile(fireProjectileInfo);
		}

		// Token: 0x06002B67 RID: 11111 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002759 RID: 10073
		public static float baseDuration;

		// Token: 0x0400275A RID: 10074
		public static GameObject projectilePrefab;

		// Token: 0x0400275B RID: 10075
		public static string fireSound;

		// Token: 0x0400275C RID: 10076
		public static float maxDistance;

		// Token: 0x0400275D RID: 10077
		public static float damageCoefficient;

		// Token: 0x0400275E RID: 10078
		public static float force;

		// Token: 0x0400275F RID: 10079
		public static string muzzleName;

		// Token: 0x04002760 RID: 10080
		public static GameObject muzzleEffect;

		// Token: 0x04002761 RID: 10081
		public static float healthCostFraction;

		// Token: 0x04002762 RID: 10082
		private float duration;
	}
}
