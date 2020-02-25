using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianBruiserMonster
{
	// Token: 0x020007EF RID: 2031
	public class Flamebreath : BaseState
	{
		// Token: 0x06002E34 RID: 11828 RVA: 0x000C4858 File Offset: 0x000C2A58
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.entryDuration = Flamebreath.baseEntryDuration / this.attackSpeedStat;
			this.exitDuration = Flamebreath.baseExitDuration / this.attackSpeedStat;
			this.flamethrowerDuration = Flamebreath.baseFlamethrowerDuration;
			Transform modelTransform = base.GetModelTransform();
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.entryDuration + this.flamethrowerDuration + 1f);
			}
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				modelTransform.GetComponent<AimAnimator>().enabled = true;
			}
			float num = this.flamethrowerDuration * Flamebreath.tickFrequency;
			this.tickDamageCoefficient = Flamebreath.totalDamageCoefficient / num;
			if (base.isAuthority && base.characterBody)
			{
				this.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			}
			base.PlayAnimation("Gesture, Override", "PrepFlamebreath", "PrepFlamebreath.playbackRate", this.entryDuration);
		}

		// Token: 0x06002E35 RID: 11829 RVA: 0x000C4960 File Offset: 0x000C2B60
		public override void OnExit()
		{
			Util.PlaySound(Flamebreath.endAttackSoundString, base.gameObject);
			base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
			if (this.flamethrowerEffectInstance)
			{
				EntityState.Destroy(this.flamethrowerEffectInstance.gameObject);
			}
			base.OnExit();
		}

		// Token: 0x06002E36 RID: 11830 RVA: 0x000C49B8 File Offset: 0x000C2BB8
		private void FireFlame(string muzzleString)
		{
			base.GetAimRay();
			if (base.isAuthority && this.muzzleTransform)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = this.muzzleTransform.position,
					aimVector = this.muzzleTransform.forward,
					minSpread = 0f,
					maxSpread = Flamebreath.maxSpread,
					damage = this.tickDamageCoefficient * this.damageStat,
					force = Flamebreath.force,
					muzzleName = muzzleString,
					hitEffectPrefab = Flamebreath.impactEffectPrefab,
					isCrit = this.isCrit,
					radius = Flamebreath.radius,
					falloffModel = BulletAttack.FalloffModel.None,
					stopperMask = LayerIndex.world.mask,
					procCoefficient = Flamebreath.procCoefficientPerTick,
					maxDistance = Flamebreath.maxDistance,
					smartCollision = true,
					damageType = (Util.CheckRoll(Flamebreath.ignitePercentChance, base.characterBody.master) ? DamageType.PercentIgniteOnHit : DamageType.Generic)
				}.Fire();
			}
		}

		// Token: 0x06002E37 RID: 11831 RVA: 0x000C4AE4 File Offset: 0x000C2CE4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.entryDuration && this.stopwatch < this.entryDuration + this.flamethrowerDuration && !this.hasBegunFlamethrower)
			{
				this.hasBegunFlamethrower = true;
				Util.PlaySound(Flamebreath.startAttackSoundString, base.gameObject);
				base.PlayAnimation("Gesture, Override", "Flamebreath", "Flamebreath.playbackRate", this.flamethrowerDuration);
				if (this.childLocator)
				{
					this.muzzleTransform = this.childLocator.FindChild("MuzzleMouth");
					this.flamethrowerEffectInstance = UnityEngine.Object.Instantiate<GameObject>(Flamebreath.flamethrowerEffectPrefab, this.muzzleTransform).transform;
					this.flamethrowerEffectInstance.transform.localPosition = Vector3.zero;
					this.flamethrowerEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
				}
			}
			if (this.stopwatch >= this.entryDuration + this.flamethrowerDuration && this.hasBegunFlamethrower)
			{
				this.hasBegunFlamethrower = false;
				base.PlayCrossfade("Gesture, Override", "ExitFlamebreath", "ExitFlamebreath.playbackRate", this.exitDuration, 0.1f);
			}
			if (this.hasBegunFlamethrower)
			{
				this.flamethrowerStopwatch += Time.deltaTime;
				if (this.flamethrowerStopwatch > 1f / Flamebreath.tickFrequency)
				{
					this.flamethrowerStopwatch -= 1f / Flamebreath.tickFrequency;
					this.FireFlame("MuzzleCenter");
				}
			}
			else if (this.flamethrowerEffectInstance)
			{
				EntityState.Destroy(this.flamethrowerEffectInstance.gameObject);
			}
			if (this.stopwatch >= this.flamethrowerDuration + this.entryDuration + this.exitDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002E38 RID: 11832 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002B44 RID: 11076
		public static GameObject flamethrowerEffectPrefab;

		// Token: 0x04002B45 RID: 11077
		public static GameObject impactEffectPrefab;

		// Token: 0x04002B46 RID: 11078
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002B47 RID: 11079
		public static float maxDistance;

		// Token: 0x04002B48 RID: 11080
		public static float radius;

		// Token: 0x04002B49 RID: 11081
		public static float baseEntryDuration = 1f;

		// Token: 0x04002B4A RID: 11082
		public static float baseExitDuration = 0.5f;

		// Token: 0x04002B4B RID: 11083
		public static float baseFlamethrowerDuration = 2f;

		// Token: 0x04002B4C RID: 11084
		public static float totalDamageCoefficient = 1.2f;

		// Token: 0x04002B4D RID: 11085
		public static float procCoefficientPerTick;

		// Token: 0x04002B4E RID: 11086
		public static float tickFrequency;

		// Token: 0x04002B4F RID: 11087
		public static float force = 20f;

		// Token: 0x04002B50 RID: 11088
		public static string startAttackSoundString;

		// Token: 0x04002B51 RID: 11089
		public static string endAttackSoundString;

		// Token: 0x04002B52 RID: 11090
		public static float ignitePercentChance;

		// Token: 0x04002B53 RID: 11091
		public static float maxSpread;

		// Token: 0x04002B54 RID: 11092
		private float tickDamageCoefficient;

		// Token: 0x04002B55 RID: 11093
		private float flamethrowerStopwatch;

		// Token: 0x04002B56 RID: 11094
		private float stopwatch;

		// Token: 0x04002B57 RID: 11095
		private float entryDuration;

		// Token: 0x04002B58 RID: 11096
		private float exitDuration;

		// Token: 0x04002B59 RID: 11097
		private float flamethrowerDuration;

		// Token: 0x04002B5A RID: 11098
		private bool hasBegunFlamethrower;

		// Token: 0x04002B5B RID: 11099
		private ChildLocator childLocator;

		// Token: 0x04002B5C RID: 11100
		private Transform flamethrowerEffectInstance;

		// Token: 0x04002B5D RID: 11101
		private Transform muzzleTransform;

		// Token: 0x04002B5E RID: 11102
		private bool isCrit;

		// Token: 0x04002B5F RID: 11103
		private const float flamethrowerEffectBaseDistance = 16f;
	}
}
