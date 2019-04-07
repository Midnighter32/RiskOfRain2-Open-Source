using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x02000113 RID: 275
	internal class FireBolt : BaseState
	{
		// Token: 0x06000546 RID: 1350 RVA: 0x000178C8 File Offset: 0x00015AC8
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = FireBolt.baseDuration / this.attackSpeedStat;
			Util.PlayScaledSound(FireBolt.attackSoundString, base.gameObject, FireBolt.attackSoundPitch);
			base.characterBody.SetAimTimer(2f);
			this.animator = base.GetModelAnimator();
			if (this.animator)
			{
				this.childLocator = this.animator.GetComponent<ChildLocator>();
			}
			FireBolt.Gauntlet gauntlet = this.gauntlet;
			if (gauntlet != FireBolt.Gauntlet.Left)
			{
				if (gauntlet != FireBolt.Gauntlet.Right)
				{
					return;
				}
				this.muzzleString = "MuzzleRight";
				if (this.attackSpeedStat < FireBolt.attackSpeedAltAnimationThreshold)
				{
					base.PlayCrossfade("Gesture, Additive", "Cast1Right", "FireGauntlet.playbackRate", this.duration, 0.1f);
					base.PlayAnimation("Gesture Left, Additive", "Empty");
					base.PlayAnimation("Gesture Right, Additive", "Empty");
					return;
				}
				base.PlayAnimation("Gesture Right, Additive", "FireGauntletRight", "FireGauntlet.playbackRate", this.duration);
				base.PlayAnimation("Gesture, Additive", "HoldGauntletsUp", "FireGauntlet.playbackRate", this.duration);
				this.FireGauntlet();
				return;
			}
			else
			{
				this.muzzleString = "MuzzleLeft";
				if (this.attackSpeedStat < FireBolt.attackSpeedAltAnimationThreshold)
				{
					base.PlayCrossfade("Gesture, Additive", "Cast1Left", "FireGauntlet.playbackRate", this.duration, 0.1f);
					base.PlayAnimation("Gesture Left, Additive", "Empty");
					base.PlayAnimation("Gesture Right, Additive", "Empty");
					return;
				}
				base.PlayAnimation("Gesture Left, Additive", "FireGauntletLeft", "FireGauntlet.playbackRate", this.duration);
				base.PlayAnimation("Gesture, Additive", "HoldGauntletsUp", "FireGauntlet.playbackRate", this.duration);
				this.FireGauntlet();
				return;
			}
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x00017A88 File Offset: 0x00015C88
		private void FireGauntlet()
		{
			if (this.hasFiredGauntlet)
			{
				return;
			}
			this.hasFiredGauntlet = true;
			Ray aimRay = base.GetAimRay();
			if (this.childLocator)
			{
				this.muzzleTransform = this.childLocator.FindChild(this.muzzleString);
			}
			if (FireBolt.muzzleflashEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireBolt.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireBolt.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, FireBolt.damageCoefficient * this.damageStat, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x00017B58 File Offset: 0x00015D58
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.animator.GetFloat("FireGauntlet.fire") > 0f && !this.hasFiredGauntlet)
			{
				this.FireGauntlet();
			}
			if (this.stopwatch < this.duration || !base.isAuthority)
			{
				return;
			}
			GenericSkill primary = base.skillLocator.primary;
			if (base.inputBank.skill1.down && primary.CanExecute())
			{
				primary.DeductStock(1);
				FireBolt fireBolt = new FireBolt();
				fireBolt.gauntlet = ((this.gauntlet == FireBolt.Gauntlet.Left) ? FireBolt.Gauntlet.Right : FireBolt.Gauntlet.Left);
				this.outer.SetNextState(fireBolt);
				return;
			}
			this.outer.SetNextStateToMain();
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x00017C16 File Offset: 0x00015E16
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.gauntlet);
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x00017C2C File Offset: 0x00015E2C
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.gauntlet = (FireBolt.Gauntlet)reader.ReadByte();
		}

		// Token: 0x0400059B RID: 1435
		public static GameObject projectilePrefab;

		// Token: 0x0400059C RID: 1436
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x0400059D RID: 1437
		public static GameObject impactEffectPrefab;

		// Token: 0x0400059E RID: 1438
		public static float procCoefficient;

		// Token: 0x0400059F RID: 1439
		public static float damageCoefficient;

		// Token: 0x040005A0 RID: 1440
		public static float force = 20f;

		// Token: 0x040005A1 RID: 1441
		public static float attackSpeedAltAnimationThreshold;

		// Token: 0x040005A2 RID: 1442
		public static float baseDuration;

		// Token: 0x040005A3 RID: 1443
		public static string attackSoundString;

		// Token: 0x040005A4 RID: 1444
		public static float attackSoundPitch;

		// Token: 0x040005A5 RID: 1445
		private float stopwatch;

		// Token: 0x040005A6 RID: 1446
		private float duration;

		// Token: 0x040005A7 RID: 1447
		private bool hasFiredGauntlet;

		// Token: 0x040005A8 RID: 1448
		private string muzzleString;

		// Token: 0x040005A9 RID: 1449
		private Transform muzzleTransform;

		// Token: 0x040005AA RID: 1450
		private Animator animator;

		// Token: 0x040005AB RID: 1451
		private ChildLocator childLocator;

		// Token: 0x040005AC RID: 1452
		public FireBolt.Gauntlet gauntlet;

		// Token: 0x02000114 RID: 276
		public enum Gauntlet
		{
			// Token: 0x040005AE RID: 1454
			Left,
			// Token: 0x040005AF RID: 1455
			Right
		}
	}
}
