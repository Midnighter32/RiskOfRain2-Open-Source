using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x0200089D RID: 2205
	public class FireTurret : BaseState
	{
		// Token: 0x06003171 RID: 12657 RVA: 0x000D4F50 File Offset: 0x000D3150
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireTurret.baseDuration / this.attackSpeedStat;
			string muzzleName = "Muzzle";
			Util.PlaySound(FireTurret.attackSoundString, base.gameObject);
			if (FireTurret.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireTurret.effectPrefab, base.gameObject, muzzleName, false);
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

		// Token: 0x06003172 RID: 12658 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003173 RID: 12659 RVA: 0x000D5064 File Offset: 0x000D3264
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

		// Token: 0x06003174 RID: 12660 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002FE0 RID: 12256
		public static GameObject effectPrefab;

		// Token: 0x04002FE1 RID: 12257
		public static GameObject hitEffectPrefab;

		// Token: 0x04002FE2 RID: 12258
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002FE3 RID: 12259
		public static string attackSoundString;

		// Token: 0x04002FE4 RID: 12260
		public static float damageCoefficient;

		// Token: 0x04002FE5 RID: 12261
		public static float force;

		// Token: 0x04002FE6 RID: 12262
		public static float minSpread;

		// Token: 0x04002FE7 RID: 12263
		public static float maxSpread;

		// Token: 0x04002FE8 RID: 12264
		public static int bulletCount;

		// Token: 0x04002FE9 RID: 12265
		public static float durationBetweenShots = 1f;

		// Token: 0x04002FEA RID: 12266
		public static float baseDuration = 2f;

		// Token: 0x04002FEB RID: 12267
		public int bulletCountCurrent = 1;

		// Token: 0x04002FEC RID: 12268
		private float duration = 2f;
	}
}
