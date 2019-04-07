using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x0200014A RID: 330
	internal class FireSpines : BaseState
	{
		// Token: 0x06000655 RID: 1621 RVA: 0x0001D9E3 File Offset: 0x0001BBE3
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireSpines.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture", "FireSpines", "FireSpines.playbackRate", this.duration);
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0001DA18 File Offset: 0x0001BC18
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.spineTimer += Time.fixedDeltaTime;
			if (this.spineTimer >= FireSpines.durationBetweenThrows / this.attackSpeedStat && this.spineCount < FireSpines.spineCountMax)
			{
				this.spineCount++;
				Ray aimRay = base.GetAimRay();
				string muzzleName = "MuzzleMouth";
				this.spineTimer -= FireSpines.durationBetweenThrows / this.attackSpeedStat;
				if (FireSpines.effectPrefab)
				{
					EffectManager.instance.SimpleMuzzleFlash(FireSpines.effectPrefab, base.gameObject, muzzleName, false);
				}
				if (base.isAuthority)
				{
					ProjectileManager.instance.FireProjectile(FireSpines.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireSpines.damageCoefficient, FireSpines.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000787 RID: 1927
		public static GameObject projectilePrefab;

		// Token: 0x04000788 RID: 1928
		public static GameObject effectPrefab;

		// Token: 0x04000789 RID: 1929
		public static float baseDuration = 2f;

		// Token: 0x0400078A RID: 1930
		public static float durationBetweenThrows = 0.1f;

		// Token: 0x0400078B RID: 1931
		public static int spineCountMax = 3;

		// Token: 0x0400078C RID: 1932
		public static float damageCoefficient = 1.2f;

		// Token: 0x0400078D RID: 1933
		public static float force = 20f;

		// Token: 0x0400078E RID: 1934
		private int spineCount;

		// Token: 0x0400078F RID: 1935
		private float spineTimer;

		// Token: 0x04000790 RID: 1936
		private float duration;
	}
}
