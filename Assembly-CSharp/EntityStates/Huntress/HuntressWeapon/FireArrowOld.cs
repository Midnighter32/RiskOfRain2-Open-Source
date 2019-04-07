using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000153 RID: 339
	internal class FireArrowOld : BaseState
	{
		// Token: 0x0600068A RID: 1674 RVA: 0x0001F2C0 File Offset: 0x0001D4C0
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireArrowOld.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "Muzzle";
			if (FireArrowOld.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireArrowOld.effectPrefab, base.gameObject, muzzleName, false);
			}
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Gesture");
				modelAnimator.SetFloat("FireArrow.playbackRate", this.attackSpeedStat);
				modelAnimator.PlayInFixedTime("FireArrow", layerIndex, 0f);
				modelAnimator.Update(0f);
				if (base.hasAuthority)
				{
					ProjectileManager.instance.FireProjectile(FireArrowOld.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireArrowOld.damageCoefficient, FireArrowOld.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
			}
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x0001F3D1 File Offset: 0x0001D5D1
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040007FA RID: 2042
		public static GameObject projectilePrefab;

		// Token: 0x040007FB RID: 2043
		public static GameObject effectPrefab;

		// Token: 0x040007FC RID: 2044
		public static float baseDuration = 2f;

		// Token: 0x040007FD RID: 2045
		public static float damageCoefficient = 1.2f;

		// Token: 0x040007FE RID: 2046
		public static float force = 20f;

		// Token: 0x040007FF RID: 2047
		private float duration;
	}
}
