using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008BE RID: 2238
	public class FireRocket : BaseState
	{
		// Token: 0x06003231 RID: 12849 RVA: 0x000D8E5C File Offset: 0x000D705C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireRocket.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "MuzzleCenter";
			base.PlayAnimation("Gesture", "FireFMJ", "FireFMJ.playbackRate", this.duration);
			if (FireRocket.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireRocket.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireRocket.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06003232 RID: 12850 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003233 RID: 12851 RVA: 0x000D8F32 File Offset: 0x000D7132
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003234 RID: 12852 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04003103 RID: 12547
		public static GameObject projectilePrefab;

		// Token: 0x04003104 RID: 12548
		public static GameObject effectPrefab;

		// Token: 0x04003105 RID: 12549
		public static float damageCoefficient;

		// Token: 0x04003106 RID: 12550
		public static float force;

		// Token: 0x04003107 RID: 12551
		public static float baseDuration = 2f;

		// Token: 0x04003108 RID: 12552
		private float duration;

		// Token: 0x04003109 RID: 12553
		public int bulletCountCurrent = 1;
	}
}
