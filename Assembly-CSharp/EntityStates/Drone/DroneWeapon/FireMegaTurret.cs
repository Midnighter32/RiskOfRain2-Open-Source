using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x0200089B RID: 2203
	public class FireMegaTurret : BaseState
	{
		// Token: 0x06003164 RID: 12644 RVA: 0x000D4A68 File Offset: 0x000D2C68
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

		// Token: 0x06003165 RID: 12645 RVA: 0x000D4AD0 File Offset: 0x000D2CD0
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
				EffectManager.SimpleMuzzleFlash(FireMegaTurret.effectPrefab, base.gameObject, muzzleString, false);
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

		// Token: 0x06003166 RID: 12646 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003167 RID: 12647 RVA: 0x000D4C0C File Offset: 0x000D2E0C
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

		// Token: 0x06003168 RID: 12648 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002FC2 RID: 12226
		public static GameObject effectPrefab;

		// Token: 0x04002FC3 RID: 12227
		public static GameObject hitEffectPrefab;

		// Token: 0x04002FC4 RID: 12228
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002FC5 RID: 12229
		public static string attackSoundString;

		// Token: 0x04002FC6 RID: 12230
		public static float attackSoundPlaybackCoefficient;

		// Token: 0x04002FC7 RID: 12231
		public static float damageCoefficient;

		// Token: 0x04002FC8 RID: 12232
		public static float force;

		// Token: 0x04002FC9 RID: 12233
		public static float minSpread;

		// Token: 0x04002FCA RID: 12234
		public static float maxSpread;

		// Token: 0x04002FCB RID: 12235
		public static int maxBulletCount;

		// Token: 0x04002FCC RID: 12236
		public static float baseTotalDuration;

		// Token: 0x04002FCD RID: 12237
		private Transform modelTransform;

		// Token: 0x04002FCE RID: 12238
		private ChildLocator childLocator;

		// Token: 0x04002FCF RID: 12239
		private float fireStopwatch;

		// Token: 0x04002FD0 RID: 12240
		private float stopwatch;

		// Token: 0x04002FD1 RID: 12241
		private float durationBetweenShots;

		// Token: 0x04002FD2 RID: 12242
		private float totalDuration;

		// Token: 0x04002FD3 RID: 12243
		private int bulletCount;
	}
}
