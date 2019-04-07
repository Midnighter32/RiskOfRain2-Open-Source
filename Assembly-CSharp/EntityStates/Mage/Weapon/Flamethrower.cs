using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x0200011A RID: 282
	internal class Flamethrower : BaseState
	{
		// Token: 0x06000567 RID: 1383 RVA: 0x00018648 File Offset: 0x00016848
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.entryDuration = Flamethrower.baseEntryDuration / this.attackSpeedStat;
			this.flamethrowerDuration = Flamethrower.baseFlamethrowerDuration;
			Transform modelTransform = base.GetModelTransform();
			MageLastElementTracker component = base.GetComponent<MageLastElementTracker>();
			if (component)
			{
				component.ApplyElement(MageElement.Fire);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.entryDuration + this.flamethrowerDuration + 1f);
			}
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				this.leftMuzzleTransform = this.childLocator.FindChild("MuzzleLeft");
				this.rightMuzzleTransform = this.childLocator.FindChild("MuzzleRight");
			}
			float num = this.flamethrowerDuration * Flamethrower.tickFrequency;
			this.tickDamageCoefficient = Flamethrower.totalDamageCoefficient / num;
			if (base.isAuthority && base.characterBody)
			{
				this.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			}
			base.PlayAnimation("Gesture, Additive", "PrepFlamethrower", "Flamethrower.playbackRate", this.entryDuration);
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x00018774 File Offset: 0x00016974
		public override void OnExit()
		{
			Util.PlaySound(Flamethrower.endAttackSoundString, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "ExitFlamethrower", 0.1f);
			if (this.leftFlamethrowerTransform)
			{
				EntityState.Destroy(this.leftFlamethrowerTransform.gameObject);
			}
			if (this.rightFlamethrowerTransform)
			{
				EntityState.Destroy(this.rightFlamethrowerTransform.gameObject);
			}
			base.OnExit();
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x000187E8 File Offset: 0x000169E8
		private void FireGauntlet(string muzzleString)
		{
			Ray aimRay = base.GetAimRay();
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = 0f,
					damage = this.tickDamageCoefficient * this.damageStat,
					force = Flamethrower.force,
					muzzleName = muzzleString,
					hitEffectPrefab = Flamethrower.impactEffectPrefab,
					isCrit = this.isCrit,
					radius = Flamethrower.radius,
					falloffModel = BulletAttack.FalloffModel.None,
					stopperMask = LayerIndex.world.mask,
					procCoefficient = Flamethrower.procCoefficientPerTick,
					maxDistance = Flamethrower.maxDistance,
					smartCollision = true,
					damageType = (Util.CheckRoll(Flamethrower.ignitePercentChance, base.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic)
				}.Fire();
				if (base.characterMotor)
				{
					base.characterMotor.ApplyForce(aimRay.direction * -Flamethrower.recoilForce, false);
				}
			}
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0001891C File Offset: 0x00016B1C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.entryDuration && !this.hasBegunFlamethrower)
			{
				this.hasBegunFlamethrower = true;
				Util.PlaySound(Flamethrower.startAttackSoundString, base.gameObject);
				base.PlayAnimation("Gesture, Additive", "Flamethrower", "Flamethrower.playbackRate", this.flamethrowerDuration);
				if (this.childLocator)
				{
					Transform parent = this.childLocator.FindChild("MuzzleLeft");
					Transform parent2 = this.childLocator.FindChild("MuzzleRight");
					this.leftFlamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(Flamethrower.flamethrowerEffectPrefab, parent).transform;
					this.rightFlamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(Flamethrower.flamethrowerEffectPrefab, parent2).transform;
					this.leftFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
					this.rightFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
				}
				this.FireGauntlet("MuzzleCenter");
			}
			if (this.hasBegunFlamethrower)
			{
				this.flamethrowerStopwatch += Time.deltaTime;
				if (this.flamethrowerStopwatch > 1f / Flamethrower.tickFrequency)
				{
					this.flamethrowerStopwatch -= 1f / Flamethrower.tickFrequency;
					this.FireGauntlet("MuzzleCenter");
				}
				this.UpdateFlamethrowerEffect();
			}
			if (this.stopwatch >= this.flamethrowerDuration + this.entryDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x00018AA4 File Offset: 0x00016CA4
		private void UpdateFlamethrowerEffect()
		{
			float num = Flamethrower.maxDistance;
			Ray aimRay = base.GetAimRay();
			Vector3 direction = aimRay.direction;
			Vector3 direction2 = aimRay.direction;
			float num2 = Flamethrower.maxDistance;
			this.leftFlamethrowerTransform.forward = direction;
			this.rightFlamethrowerTransform.forward = direction2;
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040005EF RID: 1519
		public static GameObject flamethrowerEffectPrefab;

		// Token: 0x040005F0 RID: 1520
		public static GameObject impactEffectPrefab;

		// Token: 0x040005F1 RID: 1521
		public static GameObject tracerEffectPrefab;

		// Token: 0x040005F2 RID: 1522
		public static float maxDistance;

		// Token: 0x040005F3 RID: 1523
		public static float radius;

		// Token: 0x040005F4 RID: 1524
		public static float baseEntryDuration = 1f;

		// Token: 0x040005F5 RID: 1525
		public static float baseFlamethrowerDuration = 2f;

		// Token: 0x040005F6 RID: 1526
		public static float totalDamageCoefficient = 1.2f;

		// Token: 0x040005F7 RID: 1527
		public static float procCoefficientPerTick;

		// Token: 0x040005F8 RID: 1528
		public static float tickFrequency;

		// Token: 0x040005F9 RID: 1529
		public static float force = 20f;

		// Token: 0x040005FA RID: 1530
		public static string startAttackSoundString;

		// Token: 0x040005FB RID: 1531
		public static string endAttackSoundString;

		// Token: 0x040005FC RID: 1532
		public static float ignitePercentChance;

		// Token: 0x040005FD RID: 1533
		public static float recoilForce;

		// Token: 0x040005FE RID: 1534
		private float tickDamageCoefficient;

		// Token: 0x040005FF RID: 1535
		private float flamethrowerStopwatch;

		// Token: 0x04000600 RID: 1536
		private float stopwatch;

		// Token: 0x04000601 RID: 1537
		private float entryDuration;

		// Token: 0x04000602 RID: 1538
		private float flamethrowerDuration;

		// Token: 0x04000603 RID: 1539
		private bool hasBegunFlamethrower;

		// Token: 0x04000604 RID: 1540
		private ChildLocator childLocator;

		// Token: 0x04000605 RID: 1541
		private Transform leftFlamethrowerTransform;

		// Token: 0x04000606 RID: 1542
		private Transform rightFlamethrowerTransform;

		// Token: 0x04000607 RID: 1543
		private Transform leftMuzzleTransform;

		// Token: 0x04000608 RID: 1544
		private Transform rightMuzzleTransform;

		// Token: 0x04000609 RID: 1545
		private bool isCrit;

		// Token: 0x0400060A RID: 1546
		private const float flamethrowerEffectBaseDistance = 16f;
	}
}
