using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000720 RID: 1824
	public class FireFlower2 : BaseState
	{
		// Token: 0x06002A7B RID: 10875 RVA: 0x000B2BF8 File Offset: 0x000B0DF8
		public override void OnEnter()
		{
			base.OnEnter();
			EffectManager.SimpleMuzzleFlash(FireFlower2.muzzleFlashPrefab, base.gameObject, FireFlower2.muzzleName, false);
			this.duration = FireFlower2.baseDuration / this.attackSpeedStat;
			Util.PlaySound(FireFlower2.enterSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "FireFlower", "FireFlower.playbackRate", this.duration);
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
				{
					crit = base.RollCrit(),
					damage = FireFlower2.damageCoefficient * this.damageStat,
					damageColorIndex = DamageColorIndex.Default,
					force = 0f,
					owner = base.gameObject,
					position = aimRay.origin,
					procChainMask = default(ProcChainMask),
					projectilePrefab = FireFlower2.projectilePrefab,
					rotation = Quaternion.LookRotation(aimRay.direction),
					useSpeedOverride = false
				};
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
			if (base.healthComponent)
			{
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = base.healthComponent.combinedHealth * FireFlower2.healthCostFraction;
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

		// Token: 0x06002A7C RID: 10876 RVA: 0x000B2D95 File Offset: 0x000B0F95
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002A7D RID: 10877 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002655 RID: 9813
		public static GameObject projectilePrefab;

		// Token: 0x04002656 RID: 9814
		public static float baseDuration;

		// Token: 0x04002657 RID: 9815
		public static float damageCoefficient;

		// Token: 0x04002658 RID: 9816
		public static float healthCostFraction;

		// Token: 0x04002659 RID: 9817
		public static string enterSoundString;

		// Token: 0x0400265A RID: 9818
		public static string muzzleName;

		// Token: 0x0400265B RID: 9819
		public static GameObject muzzleFlashPrefab;

		// Token: 0x0400265C RID: 9820
		private float duration;
	}
}
