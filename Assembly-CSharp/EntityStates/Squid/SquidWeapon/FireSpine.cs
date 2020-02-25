using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Squid.SquidWeapon
{
	// Token: 0x02000785 RID: 1925
	public class FireSpine : BaseState
	{
		// Token: 0x06002C33 RID: 11315 RVA: 0x000BA810 File Offset: 0x000B8A10
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireSpine.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.PlayAnimation("Gesture", "FireSpine");
			string muzzleName = "Muzzle";
			if (FireSpine.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireSpine.effectPrefab, base.gameObject, muzzleName, false);
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

		// Token: 0x06002C34 RID: 11316 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002C35 RID: 11317 RVA: 0x000BA924 File Offset: 0x000B8B24
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

		// Token: 0x06002C36 RID: 11318 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400282F RID: 10287
		public static GameObject effectPrefab;

		// Token: 0x04002830 RID: 10288
		public static GameObject hitEffectPrefab;

		// Token: 0x04002831 RID: 10289
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002832 RID: 10290
		public static float damageCoefficient;

		// Token: 0x04002833 RID: 10291
		public static float force;

		// Token: 0x04002834 RID: 10292
		public static float minSpread;

		// Token: 0x04002835 RID: 10293
		public static float maxSpread;

		// Token: 0x04002836 RID: 10294
		public static int bulletCount;

		// Token: 0x04002837 RID: 10295
		public static float durationBetweenShots = 1f;

		// Token: 0x04002838 RID: 10296
		public static float baseDuration = 2f;

		// Token: 0x04002839 RID: 10297
		public int bulletCountCurrent = 1;

		// Token: 0x0400283A RID: 10298
		private float duration;
	}
}
