using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x02000195 RID: 405
	internal class FireGatling : BaseState
	{
		// Token: 0x060007C9 RID: 1993 RVA: 0x000268A4 File Offset: 0x00024AA4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireGatling.baseDuration / this.attackSpeedStat;
			string muzzleName = "Muzzle";
			Util.PlaySound(FireGatling.fireGatlingSoundString, base.gameObject);
			base.PlayAnimation("Gesture", "FireGatling");
			if (FireGatling.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireGatling.effectPrefab, base.gameObject, muzzleName, false);
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
					minSpread = FireGatling.minSpread,
					maxSpread = FireGatling.maxSpread,
					damage = FireGatling.damageCoefficient * this.damageStat,
					force = FireGatling.force,
					tracerEffectPrefab = FireGatling.tracerEffectPrefab,
					muzzleName = muzzleName,
					hitEffectPrefab = FireGatling.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master)
				}.Fire();
			}
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x000269D0 File Offset: 0x00024BD0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= FireGatling.durationBetweenShots / this.attackSpeedStat && this.bulletCountCurrent < FireGatling.bulletCount && base.isAuthority)
			{
				FireTurret fireTurret = new FireTurret();
				fireTurret.bulletCountCurrent = this.bulletCountCurrent + 1;
				this.outer.SetNextState(fireTurret);
				return;
			}
			if (base.fixedAge >= this.duration && this.bulletCountCurrent >= FireGatling.bulletCount && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000A16 RID: 2582
		public static GameObject effectPrefab;

		// Token: 0x04000A17 RID: 2583
		public static GameObject hitEffectPrefab;

		// Token: 0x04000A18 RID: 2584
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000A19 RID: 2585
		public static float damageCoefficient;

		// Token: 0x04000A1A RID: 2586
		public static float force;

		// Token: 0x04000A1B RID: 2587
		public static float minSpread;

		// Token: 0x04000A1C RID: 2588
		public static float maxSpread;

		// Token: 0x04000A1D RID: 2589
		public static int bulletCount;

		// Token: 0x04000A1E RID: 2590
		public static float durationBetweenShots = 1f;

		// Token: 0x04000A1F RID: 2591
		public static float baseDuration = 2f;

		// Token: 0x04000A20 RID: 2592
		public static string fireGatlingSoundString;

		// Token: 0x04000A21 RID: 2593
		public int bulletCountCurrent = 1;

		// Token: 0x04000A22 RID: 2594
		private float duration;
	}
}
