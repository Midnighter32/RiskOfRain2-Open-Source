using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.LemurianMonster
{
	// Token: 0x02000125 RID: 293
	internal class FireFireball : BaseState
	{
		// Token: 0x060005A8 RID: 1448 RVA: 0x00019E28 File Offset: 0x00018028
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireFireball.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture", "FireFireball", "FireFireball.playbackRate", this.duration);
			Util.PlaySound(FireFireball.attackString, base.gameObject);
			Ray aimRay = base.GetAimRay();
			string muzzleName = "MuzzleMouth";
			if (FireFireball.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireFireball.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireFireball.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireFireball.damageCoefficient, FireFireball.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x00019F07 File Offset: 0x00018107
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000674 RID: 1652
		public static GameObject projectilePrefab;

		// Token: 0x04000675 RID: 1653
		public static GameObject effectPrefab;

		// Token: 0x04000676 RID: 1654
		public static float baseDuration = 2f;

		// Token: 0x04000677 RID: 1655
		public static float damageCoefficient = 1.2f;

		// Token: 0x04000678 RID: 1656
		public static float force = 20f;

		// Token: 0x04000679 RID: 1657
		public static string attackString;

		// Token: 0x0400067A RID: 1658
		private float duration;
	}
}
