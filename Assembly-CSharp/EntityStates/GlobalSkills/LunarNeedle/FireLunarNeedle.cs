using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.GlobalSkills.LunarNeedle
{
	// Token: 0x02000864 RID: 2148
	public class FireLunarNeedle : BaseSkillState
	{
		// Token: 0x06003076 RID: 12406 RVA: 0x000D0E00 File Offset: 0x000CF000
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireLunarNeedle.baseDuration / this.attackSpeedStat;
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, FireLunarNeedle.maxSpread, 1f, 1f, 0f, 0f);
				FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
				fireProjectileInfo.position = aimRay.origin;
				fireProjectileInfo.rotation = Quaternion.LookRotation(aimRay.direction);
				fireProjectileInfo.crit = base.characterBody.RollCrit();
				fireProjectileInfo.damage = base.characterBody.damage * FireLunarNeedle.damageCoefficient;
				fireProjectileInfo.damageColorIndex = DamageColorIndex.Default;
				fireProjectileInfo.owner = base.gameObject;
				fireProjectileInfo.procChainMask = default(ProcChainMask);
				fireProjectileInfo.force = 0f;
				fireProjectileInfo.useFuseOverride = false;
				fireProjectileInfo.useSpeedOverride = false;
				fireProjectileInfo.target = null;
				fireProjectileInfo.projectilePrefab = FireLunarNeedle.projectilePrefab;
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
			base.AddRecoil(-0.4f * FireLunarNeedle.recoilAmplitude, -0.8f * FireLunarNeedle.recoilAmplitude, -0.3f * FireLunarNeedle.recoilAmplitude, 0.3f * FireLunarNeedle.recoilAmplitude);
			base.characterBody.AddSpreadBloom(FireLunarNeedle.spreadBloomValue);
			base.StartAimMode(2f, false);
			EffectManager.SimpleMuzzleFlash(FireLunarNeedle.muzzleFlashEffectPrefab, base.gameObject, "Head", false);
			Util.PlaySound(FireLunarNeedle.fireSound, base.gameObject);
		}

		// Token: 0x06003077 RID: 12407 RVA: 0x000D0F8D File Offset: 0x000CF18D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06003078 RID: 12408 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002EC2 RID: 11970
		public static float baseDuration;

		// Token: 0x04002EC3 RID: 11971
		public static float damageCoefficient;

		// Token: 0x04002EC4 RID: 11972
		public static GameObject projectilePrefab;

		// Token: 0x04002EC5 RID: 11973
		public static float recoilAmplitude;

		// Token: 0x04002EC6 RID: 11974
		public static float spreadBloomValue;

		// Token: 0x04002EC7 RID: 11975
		public static GameObject muzzleFlashEffectPrefab;

		// Token: 0x04002EC8 RID: 11976
		public static string fireSound;

		// Token: 0x04002EC9 RID: 11977
		public static float maxSpread;

		// Token: 0x04002ECA RID: 11978
		private float duration;
	}
}
