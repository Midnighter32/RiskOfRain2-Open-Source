using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x02000117 RID: 279
	internal class FireLaserbolt : BaseState
	{
		// Token: 0x06000556 RID: 1366 RVA: 0x00017E38 File Offset: 0x00016038
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireLaserbolt.baseDuration / this.attackSpeedStat;
			FireLaserbolt.Gauntlet gauntlet = this.gauntlet;
			if (gauntlet != FireLaserbolt.Gauntlet.Left)
			{
				if (gauntlet == FireLaserbolt.Gauntlet.Right)
				{
					this.muzzleString = "MuzzleRight";
					base.PlayAnimation("Gesture Right, Additive", "FireGauntletRight", "FireGauntlet.playbackRate", this.duration);
				}
			}
			else
			{
				this.muzzleString = "MuzzleLeft";
				base.PlayAnimation("Gesture Left, Additive", "FireGauntletLeft", "FireGauntlet.playbackRate", this.duration);
			}
			base.PlayAnimation("Gesture, Additive", "HoldGauntletsUp", "FireGauntlet.playbackRate", this.duration);
			Util.PlaySound(FireLaserbolt.attackString, base.gameObject);
			this.animator = base.GetModelAnimator();
			base.characterBody.SetAimTimer(2f);
			this.FireGauntlet();
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x00017F0C File Offset: 0x0001610C
		private void FireGauntlet()
		{
			this.hasFiredGauntlet = true;
			Ray aimRay = base.GetAimRay();
			if (FireLaserbolt.muzzleEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireLaserbolt.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
			}
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = 0f,
					maxSpread = base.characterBody.spreadBloomAngle,
					damage = FireLaserbolt.damageCoefficient * this.damageStat,
					force = FireLaserbolt.force,
					tracerEffectPrefab = FireLaserbolt.tracerEffectPrefab,
					muzzleName = this.muzzleString,
					hitEffectPrefab = FireLaserbolt.impactEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					radius = 0.1f,
					smartCollision = false
				}.Fire();
			}
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x00018020 File Offset: 0x00016220
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.animator.GetFloat("FireGauntlet.fire") > 0f && !this.hasFiredGauntlet)
			{
				this.FireGauntlet();
			}
			if (base.fixedAge < this.duration || !base.isAuthority)
			{
				return;
			}
			if (base.inputBank.skill1.down)
			{
				FireLaserbolt fireLaserbolt = new FireLaserbolt();
				fireLaserbolt.gauntlet = ((this.gauntlet == FireLaserbolt.Gauntlet.Left) ? FireLaserbolt.Gauntlet.Right : FireLaserbolt.Gauntlet.Left);
				this.outer.SetNextState(fireLaserbolt);
				return;
			}
			this.outer.SetNextStateToMain();
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040005BD RID: 1469
		public static GameObject muzzleEffectPrefab;

		// Token: 0x040005BE RID: 1470
		public static GameObject tracerEffectPrefab;

		// Token: 0x040005BF RID: 1471
		public static GameObject impactEffectPrefab;

		// Token: 0x040005C0 RID: 1472
		public static float baseDuration = 2f;

		// Token: 0x040005C1 RID: 1473
		public static float damageCoefficient = 1.2f;

		// Token: 0x040005C2 RID: 1474
		public static float force = 20f;

		// Token: 0x040005C3 RID: 1475
		public static string attackString;

		// Token: 0x040005C4 RID: 1476
		private float duration;

		// Token: 0x040005C5 RID: 1477
		private bool hasFiredGauntlet;

		// Token: 0x040005C6 RID: 1478
		private string muzzleString;

		// Token: 0x040005C7 RID: 1479
		private Animator animator;

		// Token: 0x040005C8 RID: 1480
		public FireLaserbolt.Gauntlet gauntlet;

		// Token: 0x02000118 RID: 280
		public enum Gauntlet
		{
			// Token: 0x040005CA RID: 1482
			Left,
			// Token: 0x040005CB RID: 1483
			Right
		}
	}
}
