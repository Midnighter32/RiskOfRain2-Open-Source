using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianBruiserMonster
{
	// Token: 0x02000120 RID: 288
	internal class Flamebreath : BaseState
	{
		// Token: 0x0600058E RID: 1422 RVA: 0x00019558 File Offset: 0x00017758
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

		// Token: 0x0600058F RID: 1423 RVA: 0x00019660 File Offset: 0x00017860
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

		// Token: 0x06000590 RID: 1424 RVA: 0x000196B8 File Offset: 0x000178B8
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
					damageType = (Util.CheckRoll(Flamebreath.ignitePercentChance, base.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic)
				}.Fire();
			}
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x000197E4 File Offset: 0x000179E4
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

		// Token: 0x06000592 RID: 1426 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000643 RID: 1603
		public static GameObject flamethrowerEffectPrefab;

		// Token: 0x04000644 RID: 1604
		public static GameObject impactEffectPrefab;

		// Token: 0x04000645 RID: 1605
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000646 RID: 1606
		public static float maxDistance;

		// Token: 0x04000647 RID: 1607
		public static float radius;

		// Token: 0x04000648 RID: 1608
		public static float baseEntryDuration = 1f;

		// Token: 0x04000649 RID: 1609
		public static float baseExitDuration = 0.5f;

		// Token: 0x0400064A RID: 1610
		public static float baseFlamethrowerDuration = 2f;

		// Token: 0x0400064B RID: 1611
		public static float totalDamageCoefficient = 1.2f;

		// Token: 0x0400064C RID: 1612
		public static float procCoefficientPerTick;

		// Token: 0x0400064D RID: 1613
		public static float tickFrequency;

		// Token: 0x0400064E RID: 1614
		public static float force = 20f;

		// Token: 0x0400064F RID: 1615
		public static string startAttackSoundString;

		// Token: 0x04000650 RID: 1616
		public static string endAttackSoundString;

		// Token: 0x04000651 RID: 1617
		public static float ignitePercentChance;

		// Token: 0x04000652 RID: 1618
		public static float maxSpread;

		// Token: 0x04000653 RID: 1619
		private float tickDamageCoefficient;

		// Token: 0x04000654 RID: 1620
		private float flamethrowerStopwatch;

		// Token: 0x04000655 RID: 1621
		private float stopwatch;

		// Token: 0x04000656 RID: 1622
		private float entryDuration;

		// Token: 0x04000657 RID: 1623
		private float exitDuration;

		// Token: 0x04000658 RID: 1624
		private float flamethrowerDuration;

		// Token: 0x04000659 RID: 1625
		private bool hasBegunFlamethrower;

		// Token: 0x0400065A RID: 1626
		private ChildLocator childLocator;

		// Token: 0x0400065B RID: 1627
		private Transform flamethrowerEffectInstance;

		// Token: 0x0400065C RID: 1628
		private Transform muzzleTransform;

		// Token: 0x0400065D RID: 1629
		private bool isCrit;

		// Token: 0x0400065E RID: 1630
		private const float flamethrowerEffectBaseDistance = 16f;
	}
}
