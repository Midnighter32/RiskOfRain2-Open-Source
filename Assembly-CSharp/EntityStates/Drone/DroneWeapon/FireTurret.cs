using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x02000198 RID: 408
	internal class FireTurret : BaseState
	{
		// Token: 0x060007DC RID: 2012 RVA: 0x00026F7C File Offset: 0x0002517C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireTurret.baseDuration / this.attackSpeedStat;
			string muzzleName = "Muzzle";
			Util.PlaySound(FireTurret.attackSoundString, base.gameObject);
			if (FireTurret.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireTurret.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = FireTurret.minSpread,
					maxSpread = FireTurret.maxSpread,
					damage = FireTurret.damageCoefficient * this.damageStat,
					force = FireTurret.force,
					tracerEffectPrefab = FireTurret.tracerEffectPrefab,
					muzzleName = muzzleName,
					hitEffectPrefab = FireTurret.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master)
				}.Fire();
			}
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x00027098 File Offset: 0x00025298
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= FireTurret.durationBetweenShots / this.attackSpeedStat && this.bulletCountCurrent < FireTurret.bulletCount && base.isAuthority)
			{
				FireTurret fireTurret = new FireTurret();
				fireTurret.bulletCountCurrent = this.bulletCountCurrent + 1;
				this.outer.SetNextState(fireTurret);
				return;
			}
			if (base.fixedAge >= this.duration && this.bulletCountCurrent >= FireTurret.bulletCount && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000A41 RID: 2625
		public static GameObject effectPrefab;

		// Token: 0x04000A42 RID: 2626
		public static GameObject hitEffectPrefab;

		// Token: 0x04000A43 RID: 2627
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000A44 RID: 2628
		public static string attackSoundString;

		// Token: 0x04000A45 RID: 2629
		public static float damageCoefficient;

		// Token: 0x04000A46 RID: 2630
		public static float force;

		// Token: 0x04000A47 RID: 2631
		public static float minSpread;

		// Token: 0x04000A48 RID: 2632
		public static float maxSpread;

		// Token: 0x04000A49 RID: 2633
		public static int bulletCount;

		// Token: 0x04000A4A RID: 2634
		public static float durationBetweenShots = 1f;

		// Token: 0x04000A4B RID: 2635
		public static float baseDuration = 2f;

		// Token: 0x04000A4C RID: 2636
		public int bulletCountCurrent = 1;

		// Token: 0x04000A4D RID: 2637
		private float duration = 2f;
	}
}
