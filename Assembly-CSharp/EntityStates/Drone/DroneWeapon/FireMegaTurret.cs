using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x02000196 RID: 406
	internal class FireMegaTurret : BaseState
	{
		// Token: 0x060007CF RID: 1999 RVA: 0x00026A84 File Offset: 0x00024C84
		public override void OnEnter()
		{
			base.OnEnter();
			this.fireStopwatch = 0f;
			this.totalDuration = FireMegaTurret.baseTotalDuration / this.attackSpeedStat;
			this.durationBetweenShots = this.totalDuration / (float)FireMegaTurret.maxBulletCount;
			base.GetAimRay();
			Transform transform = base.GetModelTransform();
			if (transform)
			{
				this.childLocator = transform.GetComponent<ChildLocator>();
			}
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x00026AEC File Offset: 0x00024CEC
		private void FireBullet(string muzzleString)
		{
			Ray aimRay = base.GetAimRay();
			Vector3 origin = aimRay.origin;
			Util.PlayScaledSound(FireMegaTurret.attackSoundString, base.gameObject, FireMegaTurret.attackSoundPlaybackCoefficient);
			base.PlayAnimation("Gesture, Additive", "FireGat");
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild(muzzleString);
				if (transform)
				{
					Vector3 position = transform.position;
				}
			}
			if (FireMegaTurret.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireMegaTurret.effectPrefab, base.gameObject, muzzleString, false);
			}
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = FireMegaTurret.minSpread,
					maxSpread = FireMegaTurret.maxSpread,
					damage = FireMegaTurret.damageCoefficient * this.damageStat,
					force = FireMegaTurret.force,
					tracerEffectPrefab = FireMegaTurret.tracerEffectPrefab,
					muzzleName = muzzleString,
					hitEffectPrefab = FireMegaTurret.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master)
				}.Fire();
			}
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x00026C30 File Offset: 0x00024E30
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireStopwatch += Time.fixedDeltaTime;
			this.stopwatch += Time.fixedDeltaTime;
			if (this.fireStopwatch >= this.durationBetweenShots)
			{
				this.bulletCount++;
				this.fireStopwatch -= this.durationBetweenShots;
				this.FireBullet((this.bulletCount % 2 == 0) ? "GatLeft" : "GatRight");
			}
			if (this.stopwatch >= this.totalDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000A23 RID: 2595
		public static GameObject effectPrefab;

		// Token: 0x04000A24 RID: 2596
		public static GameObject hitEffectPrefab;

		// Token: 0x04000A25 RID: 2597
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000A26 RID: 2598
		public static string attackSoundString;

		// Token: 0x04000A27 RID: 2599
		public static float attackSoundPlaybackCoefficient;

		// Token: 0x04000A28 RID: 2600
		public static float damageCoefficient;

		// Token: 0x04000A29 RID: 2601
		public static float force;

		// Token: 0x04000A2A RID: 2602
		public static float minSpread;

		// Token: 0x04000A2B RID: 2603
		public static float maxSpread;

		// Token: 0x04000A2C RID: 2604
		public static int maxBulletCount;

		// Token: 0x04000A2D RID: 2605
		public static float baseTotalDuration;

		// Token: 0x04000A2E RID: 2606
		private Transform modelTransform;

		// Token: 0x04000A2F RID: 2607
		private ChildLocator childLocator;

		// Token: 0x04000A30 RID: 2608
		private float fireStopwatch;

		// Token: 0x04000A31 RID: 2609
		private float stopwatch;

		// Token: 0x04000A32 RID: 2610
		private float durationBetweenShots;

		// Token: 0x04000A33 RID: 2611
		private float totalDuration;

		// Token: 0x04000A34 RID: 2612
		private int bulletCount;
	}
}
