using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Paladin
{
	// Token: 0x020000FA RID: 250
	public class LeapSlam : BaseState
	{
		// Token: 0x060004D0 RID: 1232 RVA: 0x000145DC File Offset: 0x000127DC
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

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001463C File Offset: 0x0001283C
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

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001467C File Offset: 0x0001287C
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

		// Token: 0x060004D3 RID: 1235 RVA: 0x000148B4 File Offset: 0x00012AB4
		public override void OnExit()
		{
			if (NetworkServer.active)
			{
				this.attack.Fire(null);
			}
			if (base.isAuthority && this.modelTransform)
			{
				EffectManager.instance.SimpleMuzzleFlash(LeapSlam.slamEffectPrefab, base.gameObject, "SlamZone", true);
			}
			EntityState.Destroy(this.leftHandChargeEffect);
			EntityState.Destroy(this.rightHandChargeEffect);
			this.DisableIndicator("GroundSlamIndicator", this.modelChildLocator);
			base.OnExit();
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x00014934 File Offset: 0x00012B34
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

		// Token: 0x060004D5 RID: 1237 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040004A8 RID: 1192
		private float stopwatch;

		// Token: 0x040004A9 RID: 1193
		public static float damageCoefficient = 4f;

		// Token: 0x040004AA RID: 1194
		public static float forceMagnitude = 16f;

		// Token: 0x040004AB RID: 1195
		public static float yBias;

		// Token: 0x040004AC RID: 1196
		public static string initialAttackSoundString;

		// Token: 0x040004AD RID: 1197
		public static GameObject chargeEffectPrefab;

		// Token: 0x040004AE RID: 1198
		public static GameObject slamEffectPrefab;

		// Token: 0x040004AF RID: 1199
		public static GameObject hitEffectPrefab;

		// Token: 0x040004B0 RID: 1200
		public static float leapVelocityCoefficient;

		// Token: 0x040004B1 RID: 1201
		public static float verticalLeapBonusCoefficient;

		// Token: 0x040004B2 RID: 1202
		public static float minimumDuration;

		// Token: 0x040004B3 RID: 1203
		private float leapVelocity;

		// Token: 0x040004B4 RID: 1204
		private OverlapAttack attack;

		// Token: 0x040004B5 RID: 1205
		private Transform modelTransform;

		// Token: 0x040004B6 RID: 1206
		private GameObject leftHandChargeEffect;

		// Token: 0x040004B7 RID: 1207
		private GameObject rightHandChargeEffect;

		// Token: 0x040004B8 RID: 1208
		private ChildLocator modelChildLocator;

		// Token: 0x040004B9 RID: 1209
		private Vector3 initialAimVector;
	}
}
