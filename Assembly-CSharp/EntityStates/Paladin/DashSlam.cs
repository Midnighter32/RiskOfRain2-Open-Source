using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Paladin
{
	// Token: 0x020000F9 RID: 249
	public class DashSlam : BaseState
	{
		// Token: 0x060004C8 RID: 1224 RVA: 0x00014178 File Offset: 0x00012378
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

		// Token: 0x060004C9 RID: 1225 RVA: 0x000141D8 File Offset: 0x000123D8
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

		// Token: 0x060004CA RID: 1226 RVA: 0x00014218 File Offset: 0x00012418
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

		// Token: 0x060004CB RID: 1227 RVA: 0x000143C4 File Offset: 0x000125C4
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
				EffectManager.instance.SimpleMuzzleFlash(DashSlam.slamEffectPrefab, base.gameObject, "SlamZone", true);
			}
			EntityState.Destroy(this.leftHandChargeEffect);
			EntityState.Destroy(this.rightHandChargeEffect);
			this.DisableIndicator("GroundSlamIndicator", this.modelChildLocator);
			base.OnExit();
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x00014488 File Offset: 0x00012688
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

		// Token: 0x060004CD RID: 1229 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000495 RID: 1173
		private float stopwatch;

		// Token: 0x04000496 RID: 1174
		public static float damageCoefficient = 4f;

		// Token: 0x04000497 RID: 1175
		public static float baseForceMagnitude = 16f;

		// Token: 0x04000498 RID: 1176
		public static float bonusImpactForce;

		// Token: 0x04000499 RID: 1177
		public static string initialAttackSoundString;

		// Token: 0x0400049A RID: 1178
		public static GameObject chargeEffectPrefab;

		// Token: 0x0400049B RID: 1179
		public static GameObject slamEffectPrefab;

		// Token: 0x0400049C RID: 1180
		public static GameObject hitEffectPrefab;

		// Token: 0x0400049D RID: 1181
		public static float initialSpeedCoefficient;

		// Token: 0x0400049E RID: 1182
		public static float finalSpeedCoefficient;

		// Token: 0x0400049F RID: 1183
		public static float duration;

		// Token: 0x040004A0 RID: 1184
		public static float overlapSphereRadius;

		// Token: 0x040004A1 RID: 1185
		public static float blastAttackRadius;

		// Token: 0x040004A2 RID: 1186
		private BlastAttack attack;

		// Token: 0x040004A3 RID: 1187
		private Transform modelTransform;

		// Token: 0x040004A4 RID: 1188
		private GameObject leftHandChargeEffect;

		// Token: 0x040004A5 RID: 1189
		private GameObject rightHandChargeEffect;

		// Token: 0x040004A6 RID: 1190
		private ChildLocator modelChildLocator;

		// Token: 0x040004A7 RID: 1191
		private Vector3 initialAimVector;
	}
}
