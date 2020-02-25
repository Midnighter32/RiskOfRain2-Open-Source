using System;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x020007D2 RID: 2002
	public class FireFireBolt : BaseState, SteppedSkillDef.IStepSetter
	{
		// Token: 0x06002DA5 RID: 11685 RVA: 0x000C1C44 File Offset: 0x000BFE44
		public void SetStep(int i)
		{
			this.gauntlet = (FireFireBolt.Gauntlet)i;
		}

		// Token: 0x06002DA6 RID: 11686 RVA: 0x000C1C50 File Offset: 0x000BFE50
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = this.baseDuration / this.attackSpeedStat;
			Util.PlayScaledSound(this.attackSoundString, base.gameObject, this.attackSoundPitch);
			base.characterBody.SetAimTimer(2f);
			this.animator = base.GetModelAnimator();
			if (this.animator)
			{
				this.childLocator = this.animator.GetComponent<ChildLocator>();
			}
			FireFireBolt.Gauntlet gauntlet = this.gauntlet;
			if (gauntlet != FireFireBolt.Gauntlet.Left)
			{
				if (gauntlet != FireFireBolt.Gauntlet.Right)
				{
					return;
				}
				this.muzzleString = "MuzzleRight";
				if (this.attackSpeedStat < FireFireBolt.attackSpeedAltAnimationThreshold)
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
				if (this.attackSpeedStat < FireFireBolt.attackSpeedAltAnimationThreshold)
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

		// Token: 0x06002DA7 RID: 11687 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002DA8 RID: 11688 RVA: 0x000C1E10 File Offset: 0x000C0010
		private void FireGauntlet()
		{
			if (this.hasFiredGauntlet)
			{
				return;
			}
			base.characterBody.AddSpreadBloom(FireFireBolt.bloom);
			this.hasFiredGauntlet = true;
			Ray aimRay = base.GetAimRay();
			if (this.childLocator)
			{
				this.muzzleTransform = this.childLocator.FindChild(this.muzzleString);
			}
			if (this.muzzleflashEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(this.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageCoefficient * this.damageStat, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002DA9 RID: 11689 RVA: 0x000C1EF0 File Offset: 0x000C00F0
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
			}
		}

		// Token: 0x06002DAA RID: 11690 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06002DAB RID: 11691 RVA: 0x000C1F49 File Offset: 0x000C0149
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.gauntlet);
		}

		// Token: 0x06002DAC RID: 11692 RVA: 0x000C1F5F File Offset: 0x000C015F
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.gauntlet = (FireFireBolt.Gauntlet)reader.ReadByte();
		}

		// Token: 0x04002A58 RID: 10840
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x04002A59 RID: 10841
		[SerializeField]
		public GameObject muzzleflashEffectPrefab;

		// Token: 0x04002A5A RID: 10842
		[SerializeField]
		public float procCoefficient;

		// Token: 0x04002A5B RID: 10843
		[SerializeField]
		public float damageCoefficient;

		// Token: 0x04002A5C RID: 10844
		[SerializeField]
		public float force = 20f;

		// Token: 0x04002A5D RID: 10845
		public static float attackSpeedAltAnimationThreshold;

		// Token: 0x04002A5E RID: 10846
		[SerializeField]
		public float baseDuration;

		// Token: 0x04002A5F RID: 10847
		[SerializeField]
		public string attackSoundString;

		// Token: 0x04002A60 RID: 10848
		[SerializeField]
		public float attackSoundPitch;

		// Token: 0x04002A61 RID: 10849
		public static float bloom;

		// Token: 0x04002A62 RID: 10850
		private float stopwatch;

		// Token: 0x04002A63 RID: 10851
		private float duration;

		// Token: 0x04002A64 RID: 10852
		private bool hasFiredGauntlet;

		// Token: 0x04002A65 RID: 10853
		private string muzzleString;

		// Token: 0x04002A66 RID: 10854
		private Transform muzzleTransform;

		// Token: 0x04002A67 RID: 10855
		private Animator animator;

		// Token: 0x04002A68 RID: 10856
		private ChildLocator childLocator;

		// Token: 0x04002A69 RID: 10857
		private FireFireBolt.Gauntlet gauntlet;

		// Token: 0x020007D3 RID: 2003
		public enum Gauntlet
		{
			// Token: 0x04002A6B RID: 10859
			Left,
			// Token: 0x04002A6C RID: 10860
			Right
		}
	}
}
