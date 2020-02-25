using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x020007D8 RID: 2008
	public class FireLaserbolt : BaseState
	{
		// Token: 0x06002DB7 RID: 11703 RVA: 0x000C2170 File Offset: 0x000C0370
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

		// Token: 0x06002DB8 RID: 11704 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002DB9 RID: 11705 RVA: 0x000C2244 File Offset: 0x000C0444
		private void FireGauntlet()
		{
			this.hasFiredGauntlet = true;
			Ray aimRay = base.GetAimRay();
			if (FireLaserbolt.muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireLaserbolt.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
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

		// Token: 0x06002DBA RID: 11706 RVA: 0x000C2354 File Offset: 0x000C0554
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

		// Token: 0x06002DBB RID: 11707 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002A7A RID: 10874
		public static GameObject muzzleEffectPrefab;

		// Token: 0x04002A7B RID: 10875
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002A7C RID: 10876
		public static GameObject impactEffectPrefab;

		// Token: 0x04002A7D RID: 10877
		public static float baseDuration = 2f;

		// Token: 0x04002A7E RID: 10878
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002A7F RID: 10879
		public static float force = 20f;

		// Token: 0x04002A80 RID: 10880
		public static string attackString;

		// Token: 0x04002A81 RID: 10881
		private float duration;

		// Token: 0x04002A82 RID: 10882
		private bool hasFiredGauntlet;

		// Token: 0x04002A83 RID: 10883
		private string muzzleString;

		// Token: 0x04002A84 RID: 10884
		private Animator animator;

		// Token: 0x04002A85 RID: 10885
		public FireLaserbolt.Gauntlet gauntlet;

		// Token: 0x020007D9 RID: 2009
		public enum Gauntlet
		{
			// Token: 0x04002A87 RID: 10887
			Left,
			// Token: 0x04002A88 RID: 10888
			Right
		}
	}
}
