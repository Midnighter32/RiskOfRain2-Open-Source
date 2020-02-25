using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x0200089A RID: 2202
	public class FireGatling : BaseState
	{
		// Token: 0x0600315E RID: 12638 RVA: 0x000D4890 File Offset: 0x000D2A90
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireGatling.baseDuration / this.attackSpeedStat;
			string muzzleName = "Muzzle";
			Util.PlaySound(FireGatling.fireGatlingSoundString, base.gameObject);
			base.PlayAnimation("Gesture", "FireGatling");
			if (FireGatling.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireGatling.effectPrefab, base.gameObject, muzzleName, false);
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

		// Token: 0x0600315F RID: 12639 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003160 RID: 12640 RVA: 0x000D49B4 File Offset: 0x000D2BB4
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

		// Token: 0x06003161 RID: 12641 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002FB5 RID: 12213
		public static GameObject effectPrefab;

		// Token: 0x04002FB6 RID: 12214
		public static GameObject hitEffectPrefab;

		// Token: 0x04002FB7 RID: 12215
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002FB8 RID: 12216
		public static float damageCoefficient;

		// Token: 0x04002FB9 RID: 12217
		public static float force;

		// Token: 0x04002FBA RID: 12218
		public static float minSpread;

		// Token: 0x04002FBB RID: 12219
		public static float maxSpread;

		// Token: 0x04002FBC RID: 12220
		public static int bulletCount;

		// Token: 0x04002FBD RID: 12221
		public static float durationBetweenShots = 1f;

		// Token: 0x04002FBE RID: 12222
		public static float baseDuration = 2f;

		// Token: 0x04002FBF RID: 12223
		public static string fireGatlingSoundString;

		// Token: 0x04002FC0 RID: 12224
		public int bulletCountCurrent = 1;

		// Token: 0x04002FC1 RID: 12225
		private float duration;
	}
}
