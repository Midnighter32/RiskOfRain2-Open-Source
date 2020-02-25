using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000835 RID: 2101
	public class FireArrowOld : BaseState
	{
		// Token: 0x06002F8B RID: 12171 RVA: 0x000CB980 File Offset: 0x000C9B80
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireArrowOld.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "Muzzle";
			if (FireArrowOld.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireArrowOld.effectPrefab, base.gameObject, muzzleName, false);
			}
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Gesture");
				modelAnimator.SetFloat("FireArrow.playbackRate", this.attackSpeedStat);
				modelAnimator.PlayInFixedTime("FireArrow", layerIndex, 0f);
				modelAnimator.Update(0f);
				if (base.isAuthority)
				{
					ProjectileManager.instance.FireProjectile(FireArrowOld.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireArrowOld.damageCoefficient, FireArrowOld.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
			}
		}

		// Token: 0x06002F8C RID: 12172 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002F8D RID: 12173 RVA: 0x000CBA8C File Offset: 0x000C9C8C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002F8E RID: 12174 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002D40 RID: 11584
		public static GameObject projectilePrefab;

		// Token: 0x04002D41 RID: 11585
		public static GameObject effectPrefab;

		// Token: 0x04002D42 RID: 11586
		public static float baseDuration = 2f;

		// Token: 0x04002D43 RID: 11587
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002D44 RID: 11588
		public static float force = 20f;

		// Token: 0x04002D45 RID: 11589
		private float duration;
	}
}
