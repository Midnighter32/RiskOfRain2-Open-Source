using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Paladin
{
	// Token: 0x020007AA RID: 1962
	public class DashSlam : BaseState
	{
		// Token: 0x06002CDC RID: 11484 RVA: 0x000BD3BC File Offset: 0x000BB5BC
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

		// Token: 0x06002CDD RID: 11485 RVA: 0x000BD41C File Offset: 0x000BB61C
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

		// Token: 0x06002CDE RID: 11486 RVA: 0x000BD45C File Offset: 0x000BB65C
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelTransform = base.GetModelTransform();
			Util.PlaySound(DashSlam.initialAttackSoundString, base.gameObject);
			this.initialAimVector = Vector3.ProjectOnPlane(base.GetAimRay().direction, Vector3.up);
			base.characterMotor.velocity.y = 0f;
			base.characterDirection.forward = this.initialAimVector;
			this.attack = new BlastAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.baseDamage = DashSlam.damageCoefficient * this.damageStat;
			this.attack.damageType = DamageType.Stun1s;
			this.attack.baseForce = DashSlam.baseForceMagnitude;
			this.attack.radius = DashSlam.blastAttackRadius + base.characterBody.radius;
			this.attack.falloffModel = BlastAttack.FalloffModel.None;
			if (this.modelTransform)
			{
				this.modelChildLocator = this.modelTransform.GetComponent<ChildLocator>();
				if (this.modelChildLocator)
				{
					GameObject original = DashSlam.chargeEffectPrefab;
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

		// Token: 0x06002CDF RID: 11487 RVA: 0x000BD608 File Offset: 0x000BB808
		public override void OnExit()
		{
			if (NetworkServer.active)
			{
				this.attack.position = base.transform.position;
				this.attack.bonusForce = (this.initialAimVector + Vector3.up * 0.3f) * DashSlam.bonusImpactForce;
				this.attack.Fire();
			}
			if (base.isAuthority && this.modelTransform)
			{
				EffectManager.SimpleMuzzleFlash(DashSlam.slamEffectPrefab, base.gameObject, "SlamZone", true);
			}
			EntityState.Destroy(this.leftHandChargeEffect);
			EntityState.Destroy(this.rightHandChargeEffect);
			this.DisableIndicator("GroundSlamIndicator", this.modelChildLocator);
			base.OnExit();
		}

		// Token: 0x06002CE0 RID: 11488 RVA: 0x000BD6C8 File Offset: 0x000BB8C8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (base.isAuthority)
			{
				Collider[] array = Physics.OverlapSphere(base.transform.position, base.characterBody.radius + DashSlam.overlapSphereRadius, LayerIndex.entityPrecise.mask);
				for (int i = 0; i < array.Length; i++)
				{
					HurtBox component = array[i].GetComponent<HurtBox>();
					if (component && component.healthComponent != base.healthComponent)
					{
						this.outer.SetNextStateToMain();
						return;
					}
				}
			}
			if (base.characterMotor)
			{
				float num = Mathf.Lerp(DashSlam.initialSpeedCoefficient, DashSlam.finalSpeedCoefficient, this.stopwatch / DashSlam.duration) * base.characterBody.moveSpeed;
				Vector3 velocity = new Vector3(this.initialAimVector.x * num, 0f, this.initialAimVector.z * num);
				base.characterMotor.velocity = velocity;
				base.characterMotor.moveDirection = this.initialAimVector;
			}
			if (this.stopwatch > DashSlam.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002CE1 RID: 11489 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040028FE RID: 10494
		private float stopwatch;

		// Token: 0x040028FF RID: 10495
		public static float damageCoefficient = 4f;

		// Token: 0x04002900 RID: 10496
		public static float baseForceMagnitude = 16f;

		// Token: 0x04002901 RID: 10497
		public static float bonusImpactForce;

		// Token: 0x04002902 RID: 10498
		public static string initialAttackSoundString;

		// Token: 0x04002903 RID: 10499
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002904 RID: 10500
		public static GameObject slamEffectPrefab;

		// Token: 0x04002905 RID: 10501
		public static GameObject hitEffectPrefab;

		// Token: 0x04002906 RID: 10502
		public static float initialSpeedCoefficient;

		// Token: 0x04002907 RID: 10503
		public static float finalSpeedCoefficient;

		// Token: 0x04002908 RID: 10504
		public static float duration;

		// Token: 0x04002909 RID: 10505
		public static float overlapSphereRadius;

		// Token: 0x0400290A RID: 10506
		public static float blastAttackRadius;

		// Token: 0x0400290B RID: 10507
		private BlastAttack attack;

		// Token: 0x0400290C RID: 10508
		private Transform modelTransform;

		// Token: 0x0400290D RID: 10509
		private GameObject leftHandChargeEffect;

		// Token: 0x0400290E RID: 10510
		private GameObject rightHandChargeEffect;

		// Token: 0x0400290F RID: 10511
		private ChildLocator modelChildLocator;

		// Token: 0x04002910 RID: 10512
		private Vector3 initialAimVector;
	}
}
