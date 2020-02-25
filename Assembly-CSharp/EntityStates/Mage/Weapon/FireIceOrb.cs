using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x020007D6 RID: 2006
	public class FireIceOrb : BaseState
	{
		// Token: 0x06002DB0 RID: 11696 RVA: 0x000C1F90 File Offset: 0x000C0190
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireIceOrb.baseDuration / this.attackSpeedStat;
			FireIceOrb.Gauntlet gauntlet = this.gauntlet;
			if (gauntlet != FireIceOrb.Gauntlet.Left)
			{
				if (gauntlet == FireIceOrb.Gauntlet.Right)
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
			Util.PlaySound(FireIceOrb.attackString, base.gameObject);
			this.animator = base.GetModelAnimator();
			base.characterBody.SetAimTimer(2f);
			this.FireGauntlet();
		}

		// Token: 0x06002DB1 RID: 11697 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002DB2 RID: 11698 RVA: 0x000C2064 File Offset: 0x000C0264
		private void FireGauntlet()
		{
			this.hasFiredGauntlet = true;
			Ray aimRay = base.GetAimRay();
			if (FireIceOrb.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireIceOrb.effectPrefab, base.gameObject, this.muzzleString, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireIceOrb.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireIceOrb.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002DB3 RID: 11699 RVA: 0x000C2100 File Offset: 0x000C0300
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.animator.GetFloat("FireGauntlet.fire") > 0f && !this.hasFiredGauntlet)
			{
				this.FireGauntlet();
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002DB4 RID: 11700 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002A6D RID: 10861
		public static GameObject projectilePrefab;

		// Token: 0x04002A6E RID: 10862
		public static GameObject effectPrefab;

		// Token: 0x04002A6F RID: 10863
		public static float baseDuration = 2f;

		// Token: 0x04002A70 RID: 10864
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002A71 RID: 10865
		public static string attackString;

		// Token: 0x04002A72 RID: 10866
		private float duration;

		// Token: 0x04002A73 RID: 10867
		private bool hasFiredGauntlet;

		// Token: 0x04002A74 RID: 10868
		private string muzzleString;

		// Token: 0x04002A75 RID: 10869
		private Animator animator;

		// Token: 0x04002A76 RID: 10870
		public FireIceOrb.Gauntlet gauntlet;

		// Token: 0x020007D7 RID: 2007
		public enum Gauntlet
		{
			// Token: 0x04002A78 RID: 10872
			Left,
			// Token: 0x04002A79 RID: 10873
			Right
		}
	}
}
