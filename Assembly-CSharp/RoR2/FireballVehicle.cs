using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000201 RID: 513
	[RequireComponent(typeof(VehicleSeat))]
	[RequireComponent(typeof(Rigidbody))]
	public class FireballVehicle : MonoBehaviour, ICameraStateProvider
	{
		// Token: 0x06000AF3 RID: 2803 RVA: 0x0003062C File Offset: 0x0002E82C
		private void Awake()
		{
			this.vehicleSeat = base.GetComponent<VehicleSeat>();
			this.vehicleSeat.onPassengerEnter += this.OnPassengerEnter;
			this.vehicleSeat.onPassengerExit += this.OnPassengerExit;
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x00030680 File Offset: 0x0002E880
		private void OnPassengerExit(GameObject passenger)
		{
			if (NetworkServer.active)
			{
				this.DetonateServer();
			}
			foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
			{
				if (cameraRigController.target == passenger)
				{
					cameraRigController.SetOverrideCam(this, 0f);
					cameraRigController.SetOverrideCam(null, this.cameraLerpTime);
				}
			}
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x000306FC File Offset: 0x0002E8FC
		private void OnPassengerEnter(GameObject passenger)
		{
			if (!this.vehicleSeat.currentPassengerInputBank)
			{
				return;
			}
			Vector3 aimDirection = this.vehicleSeat.currentPassengerInputBank.aimDirection;
			this.rigidbody.rotation = Quaternion.LookRotation(aimDirection);
			this.rigidbody.velocity = aimDirection * this.initialSpeed;
			CharacterBody currentPassengerBody = this.vehicleSeat.currentPassengerBody;
			this.overlapAttack = new OverlapAttack
			{
				attacker = currentPassengerBody.gameObject,
				damage = this.overlapDamageCoefficient * currentPassengerBody.damage,
				pushAwayForce = this.overlapForce,
				isCrit = currentPassengerBody.RollCrit(),
				damageColorIndex = DamageColorIndex.Item,
				inflictor = base.gameObject,
				procChainMask = default(ProcChainMask),
				procCoefficient = this.overlapProcCoefficient,
				teamIndex = currentPassengerBody.teamComponent.teamIndex,
				hitBoxGroup = base.gameObject.GetComponent<HitBoxGroup>(),
				hitEffectPrefab = this.overlapHitEffectPrefab
			};
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x000307FC File Offset: 0x0002E9FC
		private void DetonateServer()
		{
			if (this.hasDetonatedServer)
			{
				return;
			}
			this.hasDetonatedServer = true;
			CharacterBody currentPassengerBody = this.vehicleSeat.currentPassengerBody;
			if (currentPassengerBody)
			{
				EffectData effectData = new EffectData
				{
					origin = base.transform.position,
					scale = this.blastRadius
				};
				EffectManager.SpawnEffect(this.explosionEffectPrefab, effectData, true);
				new BlastAttack
				{
					attacker = currentPassengerBody.gameObject,
					baseDamage = this.blastDamageCoefficient * currentPassengerBody.damage,
					baseForce = this.blastForce,
					bonusForce = this.blastBonusForce,
					canHurtAttacker = false,
					crit = currentPassengerBody.RollCrit(),
					damageColorIndex = DamageColorIndex.Item,
					damageType = this.blastDamageType,
					falloffModel = this.blastFalloffModel,
					inflictor = base.gameObject,
					position = base.transform.position,
					procChainMask = default(ProcChainMask),
					procCoefficient = this.blastProcCoefficient,
					radius = this.blastRadius,
					teamIndex = currentPassengerBody.teamComponent.teamIndex
				}.Fire();
			}
			Util.PlaySound(this.explosionSoundString, base.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x00030944 File Offset: 0x0002EB44
		private void FixedUpdate()
		{
			if (!this.vehicleSeat)
			{
				return;
			}
			if (!this.vehicleSeat.currentPassengerInputBank)
			{
				return;
			}
			this.age += Time.fixedDeltaTime;
			this.overlapFireAge += Time.fixedDeltaTime;
			this.overlapResetAge += Time.fixedDeltaTime;
			if (NetworkServer.active)
			{
				if (this.overlapFireAge > 1f / this.overlapFireFrequency)
				{
					if (this.overlapAttack.Fire(null))
					{
						this.age = Mathf.Max(0f, this.age - this.overlapVehicleDurationBonusPerHit);
					}
					this.overlapFireAge = 0f;
				}
				if (this.overlapResetAge >= 1f / this.overlapResetFrequency)
				{
					this.overlapAttack.ResetIgnoredHealthComponents();
					this.overlapResetAge = 0f;
				}
			}
			Ray originalAimRay = this.vehicleSeat.currentPassengerInputBank.GetAimRay();
			float num;
			originalAimRay = CameraRigController.ModifyAimRayIfApplicable(originalAimRay, base.gameObject, out num);
			Vector3 velocity = this.rigidbody.velocity;
			Vector3 target = originalAimRay.direction * this.targetSpeed;
			Vector3 a = Vector3.MoveTowards(velocity, target, this.acceleration * Time.fixedDeltaTime);
			this.rigidbody.MoveRotation(Quaternion.LookRotation(originalAimRay.direction));
			this.rigidbody.AddForce(a - velocity, ForceMode.VelocityChange);
			if (NetworkServer.active && this.duration <= this.age)
			{
				this.DetonateServer();
			}
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x00030ABD File Offset: 0x0002ECBD
		private void OnCollisionEnter(Collision collision)
		{
			if (this.detonateOnCollision && NetworkServer.active)
			{
				this.DetonateServer();
			}
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x0000409B File Offset: 0x0000229B
		public void GetCameraState(CameraRigController cameraRigController, ref CameraState cameraState)
		{
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool AllowUserLook(CameraRigController cameraRigController)
		{
			return false;
		}

		// Token: 0x04000B44 RID: 2884
		[Header("Vehicle Parameters")]
		public float duration = 3f;

		// Token: 0x04000B45 RID: 2885
		public float initialSpeed = 120f;

		// Token: 0x04000B46 RID: 2886
		public float targetSpeed = 40f;

		// Token: 0x04000B47 RID: 2887
		public float acceleration = 20f;

		// Token: 0x04000B48 RID: 2888
		public float cameraLerpTime = 1f;

		// Token: 0x04000B49 RID: 2889
		[Header("Blast Parameters")]
		public bool detonateOnCollision;

		// Token: 0x04000B4A RID: 2890
		public GameObject explosionEffectPrefab;

		// Token: 0x04000B4B RID: 2891
		public float blastDamageCoefficient;

		// Token: 0x04000B4C RID: 2892
		public float blastRadius;

		// Token: 0x04000B4D RID: 2893
		public float blastForce;

		// Token: 0x04000B4E RID: 2894
		public BlastAttack.FalloffModel blastFalloffModel;

		// Token: 0x04000B4F RID: 2895
		public DamageType blastDamageType;

		// Token: 0x04000B50 RID: 2896
		public Vector3 blastBonusForce;

		// Token: 0x04000B51 RID: 2897
		public float blastProcCoefficient;

		// Token: 0x04000B52 RID: 2898
		public string explosionSoundString;

		// Token: 0x04000B53 RID: 2899
		[Header("Overlap Parameters")]
		public float overlapDamageCoefficient;

		// Token: 0x04000B54 RID: 2900
		public float overlapProcCoefficient;

		// Token: 0x04000B55 RID: 2901
		public float overlapForce;

		// Token: 0x04000B56 RID: 2902
		public float overlapFireFrequency;

		// Token: 0x04000B57 RID: 2903
		public float overlapResetFrequency;

		// Token: 0x04000B58 RID: 2904
		public float overlapVehicleDurationBonusPerHit;

		// Token: 0x04000B59 RID: 2905
		public GameObject overlapHitEffectPrefab;

		// Token: 0x04000B5A RID: 2906
		private float age;

		// Token: 0x04000B5B RID: 2907
		private bool hasDetonatedServer;

		// Token: 0x04000B5C RID: 2908
		private VehicleSeat vehicleSeat;

		// Token: 0x04000B5D RID: 2909
		private Rigidbody rigidbody;

		// Token: 0x04000B5E RID: 2910
		private OverlapAttack overlapAttack;

		// Token: 0x04000B5F RID: 2911
		private float overlapFireAge;

		// Token: 0x04000B60 RID: 2912
		private float overlapResetAge;
	}
}
