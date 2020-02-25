using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x020007DB RID: 2011
	public class Flamethrower : BaseState
	{
		// Token: 0x06002DC8 RID: 11720 RVA: 0x000C2978 File Offset: 0x000C0B78
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.entryDuration = Flamethrower.baseEntryDuration / this.attackSpeedStat;
			this.flamethrowerDuration = Flamethrower.baseFlamethrowerDuration;
			Transform modelTransform = base.GetModelTransform();
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

		// Token: 0x06002DC9 RID: 11721 RVA: 0x000C2A8C File Offset: 0x000C0C8C
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

		// Token: 0x06002DCA RID: 11722 RVA: 0x000C2B00 File Offset: 0x000C0D00
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
					maxDistance = this.maxDistance,
					smartCollision = true,
					damageType = (Util.CheckRoll(Flamethrower.ignitePercentChance, base.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic)
				}.Fire();
				if (base.characterMotor)
				{
					base.characterMotor.ApplyForce(aimRay.direction * -Flamethrower.recoilForce, false, false);
				}
			}
		}

		// Token: 0x06002DCB RID: 11723 RVA: 0x000C2C38 File Offset: 0x000C0E38
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
					Transform transform = this.childLocator.FindChild("MuzzleLeft");
					Transform transform2 = this.childLocator.FindChild("MuzzleRight");
					if (transform)
					{
						this.leftFlamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(this.flamethrowerEffectPrefab, transform).transform;
					}
					if (transform2)
					{
						this.rightFlamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(this.flamethrowerEffectPrefab, transform2).transform;
					}
					if (this.leftFlamethrowerTransform)
					{
						this.leftFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
					}
					if (this.rightFlamethrowerTransform)
					{
						this.rightFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
					}
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

		// Token: 0x06002DCC RID: 11724 RVA: 0x000C2DEC File Offset: 0x000C0FEC
		private void UpdateFlamethrowerEffect()
		{
			Ray aimRay = base.GetAimRay();
			Vector3 direction = aimRay.direction;
			Vector3 direction2 = aimRay.direction;
			if (this.leftFlamethrowerTransform)
			{
				this.leftFlamethrowerTransform.forward = direction;
			}
			if (this.rightFlamethrowerTransform)
			{
				this.rightFlamethrowerTransform.forward = direction2;
			}
		}

		// Token: 0x06002DCD RID: 11725 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002AAC RID: 10924
		[SerializeField]
		public GameObject flamethrowerEffectPrefab;

		// Token: 0x04002AAD RID: 10925
		public static GameObject impactEffectPrefab;

		// Token: 0x04002AAE RID: 10926
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002AAF RID: 10927
		[SerializeField]
		public float maxDistance;

		// Token: 0x04002AB0 RID: 10928
		public static float radius;

		// Token: 0x04002AB1 RID: 10929
		public static float baseEntryDuration = 1f;

		// Token: 0x04002AB2 RID: 10930
		public static float baseFlamethrowerDuration = 2f;

		// Token: 0x04002AB3 RID: 10931
		public static float totalDamageCoefficient = 1.2f;

		// Token: 0x04002AB4 RID: 10932
		public static float procCoefficientPerTick;

		// Token: 0x04002AB5 RID: 10933
		public static float tickFrequency;

		// Token: 0x04002AB6 RID: 10934
		public static float force = 20f;

		// Token: 0x04002AB7 RID: 10935
		public static string startAttackSoundString;

		// Token: 0x04002AB8 RID: 10936
		public static string endAttackSoundString;

		// Token: 0x04002AB9 RID: 10937
		public static float ignitePercentChance;

		// Token: 0x04002ABA RID: 10938
		public static float recoilForce;

		// Token: 0x04002ABB RID: 10939
		private float tickDamageCoefficient;

		// Token: 0x04002ABC RID: 10940
		private float flamethrowerStopwatch;

		// Token: 0x04002ABD RID: 10941
		private float stopwatch;

		// Token: 0x04002ABE RID: 10942
		private float entryDuration;

		// Token: 0x04002ABF RID: 10943
		private float flamethrowerDuration;

		// Token: 0x04002AC0 RID: 10944
		private bool hasBegunFlamethrower;

		// Token: 0x04002AC1 RID: 10945
		private ChildLocator childLocator;

		// Token: 0x04002AC2 RID: 10946
		private Transform leftFlamethrowerTransform;

		// Token: 0x04002AC3 RID: 10947
		private Transform rightFlamethrowerTransform;

		// Token: 0x04002AC4 RID: 10948
		private Transform leftMuzzleTransform;

		// Token: 0x04002AC5 RID: 10949
		private Transform rightMuzzleTransform;

		// Token: 0x04002AC6 RID: 10950
		private bool isCrit;

		// Token: 0x04002AC7 RID: 10951
		private const float flamethrowerEffectBaseDistance = 16f;
	}
}
