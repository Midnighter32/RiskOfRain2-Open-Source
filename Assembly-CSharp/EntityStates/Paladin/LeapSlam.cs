using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Paladin
{
	// Token: 0x020007AB RID: 1963
	public class LeapSlam : BaseState
	{
		// Token: 0x06002CE4 RID: 11492 RVA: 0x000BD81C File Offset: 0x000BBA1C
		private void EnableIndicator(string childLocatorName, ChildLocator childLocator = null)
		{
			if (!childLocator)
			{
				childLocator = base.GetModelTransform().GetComponent<ChildLocator>();
			}
			Transform transform = childLocator.FindChild(childLocatorName);
			if (transform)
			{
				transform.gameObject.SetActive(true);
				ObjectScaleCurve component = transform.gameObject.GetComponent<ObjectScaleCurve>();
				if (component)
				{
					component.time = 0f;
				}
			}
		}

		// Token: 0x06002CE5 RID: 11493 RVA: 0x000BD87C File Offset: 0x000BBA7C
		private void DisableIndicator(string childLocatorName, ChildLocator childLocator = null)
		{
			if (!childLocator)
			{
				childLocator = base.GetModelTransform().GetComponent<ChildLocator>();
			}
			Transform transform = childLocator.FindChild(childLocatorName);
			if (transform)
			{
				transform.gameObject.SetActive(false);
			}
		}

		// Token: 0x06002CE6 RID: 11494 RVA: 0x000BD8BC File Offset: 0x000BBABC
		public override void OnEnter()
		{
			base.OnEnter();
			this.leapVelocity = base.characterBody.moveSpeed * LeapSlam.leapVelocityCoefficient;
			this.modelTransform = base.GetModelTransform();
			Util.PlaySound(LeapSlam.initialAttackSoundString, base.gameObject);
			this.initialAimVector = base.GetAimRay().direction;
			this.initialAimVector.y = Mathf.Max(this.initialAimVector.y, 0f);
			this.initialAimVector.y = this.initialAimVector.y + LeapSlam.yBias;
			this.initialAimVector = this.initialAimVector.normalized;
			base.characterMotor.velocity.y = this.leapVelocity * this.initialAimVector.y * LeapSlam.verticalLeapBonusCoefficient;
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = LeapSlam.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = LeapSlam.hitEffectPrefab;
			this.attack.damageType = DamageType.Stun1s;
			this.attack.forceVector = Vector3.up * LeapSlam.forceMagnitude;
			if (this.modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "GroundSlam");
			}
			if (this.modelTransform)
			{
				this.modelChildLocator = this.modelTransform.GetComponent<ChildLocator>();
				if (this.modelChildLocator)
				{
					GameObject original = LeapSlam.chargeEffectPrefab;
					Transform transform = this.modelChildLocator.FindChild("HandL");
					Transform transform2 = this.modelChildLocator.FindChild("HandR");
					if (transform)
					{
						this.leftHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform);
					}
					if (transform2)
					{
						this.rightHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform2);
					}
					this.EnableIndicator("GroundSlamIndicator", this.modelChildLocator);
				}
			}
		}

		// Token: 0x06002CE7 RID: 11495 RVA: 0x000BDAF4 File Offset: 0x000BBCF4
		public override void OnExit()
		{
			if (NetworkServer.active)
			{
				this.attack.Fire(null);
			}
			if (base.isAuthority && this.modelTransform)
			{
				EffectManager.SimpleMuzzleFlash(LeapSlam.slamEffectPrefab, base.gameObject, "SlamZone", true);
			}
			EntityState.Destroy(this.leftHandChargeEffect);
			EntityState.Destroy(this.rightHandChargeEffect);
			this.DisableIndicator("GroundSlamIndicator", this.modelChildLocator);
			base.OnExit();
		}

		// Token: 0x06002CE8 RID: 11496 RVA: 0x000BDB70 File Offset: 0x000BBD70
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (base.characterMotor)
			{
				Vector3 velocity = base.characterMotor.velocity;
				Vector3 velocity2 = new Vector3(this.initialAimVector.x * this.leapVelocity, velocity.y, this.initialAimVector.z * this.leapVelocity);
				base.characterMotor.velocity = velocity2;
				base.characterMotor.moveDirection = this.initialAimVector;
			}
			if (base.characterMotor.isGrounded && this.stopwatch > LeapSlam.minimumDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002CE9 RID: 11497 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002911 RID: 10513
		private float stopwatch;

		// Token: 0x04002912 RID: 10514
		public static float damageCoefficient = 4f;

		// Token: 0x04002913 RID: 10515
		public static float forceMagnitude = 16f;

		// Token: 0x04002914 RID: 10516
		public static float yBias;

		// Token: 0x04002915 RID: 10517
		public static string initialAttackSoundString;

		// Token: 0x04002916 RID: 10518
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002917 RID: 10519
		public static GameObject slamEffectPrefab;

		// Token: 0x04002918 RID: 10520
		public static GameObject hitEffectPrefab;

		// Token: 0x04002919 RID: 10521
		public static float leapVelocityCoefficient;

		// Token: 0x0400291A RID: 10522
		public static float verticalLeapBonusCoefficient;

		// Token: 0x0400291B RID: 10523
		public static float minimumDuration;

		// Token: 0x0400291C RID: 10524
		private float leapVelocity;

		// Token: 0x0400291D RID: 10525
		private OverlapAttack attack;

		// Token: 0x0400291E RID: 10526
		private Transform modelTransform;

		// Token: 0x0400291F RID: 10527
		private GameObject leftHandChargeEffect;

		// Token: 0x04002920 RID: 10528
		private GameObject rightHandChargeEffect;

		// Token: 0x04002921 RID: 10529
		private ChildLocator modelChildLocator;

		// Token: 0x04002922 RID: 10530
		private Vector3 initialAimVector;
	}
}
