using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Squid.SquidWeapon
{
	// Token: 0x020000F4 RID: 244
	internal class FireSpine : BaseState
	{
		// Token: 0x060004A9 RID: 1193 RVA: 0x000135D8 File Offset: 0x000117D8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireSpine.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.PlayAnimation("Gesture", "FireSpine");
			string muzzleName = "Muzzle";
			if (FireSpine.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireSpine.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = FireSpine.minSpread,
					maxSpread = FireSpine.maxSpread,
					damage = FireSpine.damageCoefficient * this.damageStat,
					force = FireSpine.force,
					tracerEffectPrefab = FireSpine.tracerEffectPrefab,
					muzzleName = muzzleName,
					hitEffectPrefab = FireSpine.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master)
				}.Fire();
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x000136F0 File Offset: 0x000118F0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= FireSpine.durationBetweenShots / this.attackSpeedStat && this.bulletCountCurrent < FireSpine.bulletCount && base.isAuthority)
			{
				FireSpine fireSpine = new FireSpine();
				fireSpine.bulletCountCurrent = this.bulletCountCurrent + 1;
				this.outer.SetNextState(fireSpine);
				return;
			}
			if (base.fixedAge >= this.duration && this.bulletCountCurrent >= FireSpine.bulletCount && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400045D RID: 1117
		public static GameObject effectPrefab;

		// Token: 0x0400045E RID: 1118
		public static GameObject hitEffectPrefab;

		// Token: 0x0400045F RID: 1119
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000460 RID: 1120
		public static float damageCoefficient;

		// Token: 0x04000461 RID: 1121
		public static float force;

		// Token: 0x04000462 RID: 1122
		public static float minSpread;

		// Token: 0x04000463 RID: 1123
		public static float maxSpread;

		// Token: 0x04000464 RID: 1124
		public static int bulletCount;

		// Token: 0x04000465 RID: 1125
		public static float durationBetweenShots = 1f;

		// Token: 0x04000466 RID: 1126
		public static float baseDuration = 2f;

		// Token: 0x04000467 RID: 1127
		public int bulletCountCurrent = 1;

		// Token: 0x04000468 RID: 1128
		private float duration;
	}
}
