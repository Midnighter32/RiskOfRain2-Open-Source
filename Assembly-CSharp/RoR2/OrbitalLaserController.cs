using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002B2 RID: 690
	public class OrbitalLaserController : MonoBehaviour
	{
		// Token: 0x06000FA2 RID: 4002 RVA: 0x000448FE File Offset: 0x00042AFE
		private void Start()
		{
			this.chargeEffect.SetActive(true);
			this.chargeEffect.GetComponent<ObjectScaleCurve>().timeMax = this.chargeDuration;
			this.mostRecentPointerPosition = base.transform.position;
			this.mostRecentPointerNormal = Vector3.up;
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x00044940 File Offset: 0x00042B40
		private void UpdateLaserPointer()
		{
			if (this.ownerBody)
			{
				this.ownerInputBank = this.ownerBody.GetComponent<InputBankTest>();
				Ray ray = new Ray
				{
					origin = this.ownerInputBank.aimOrigin,
					direction = this.ownerInputBank.aimDirection
				};
				this.mostRecentMuzzlePosition = ray.origin;
				float num = 900f;
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, num, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
				{
					this.mostRecentPointerPosition = raycastHit.point;
				}
				else
				{
					this.mostRecentPointerPosition = ray.GetPoint(num);
				}
				this.mostRecentPointerNormal = -ray.direction;
			}
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x00044A11 File Offset: 0x00042C11
		private void Update()
		{
			this.UpdateLaserPointer();
			this.laserPointerEffectTransform.SetPositionAndRotation(this.mostRecentPointerPosition, Quaternion.LookRotation(this.mostRecentPointerNormal));
			this.muzzleEffectTransform.SetPositionAndRotation(this.mostRecentMuzzlePosition, Quaternion.identity);
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x00044A4C File Offset: 0x00042C4C
		private void FixedUpdate()
		{
			this.UpdateLaserPointer();
			if (NetworkServer.active)
			{
				base.transform.position = Vector3.SmoothDamp(base.transform.position, this.mostRecentPointerPosition, ref this.velocity, this.smoothDampTime, this.maxSpeed, Time.fixedDeltaTime);
			}
		}

		// Token: 0x04000F0E RID: 3854
		[NonSerialized]
		public CharacterBody ownerBody;

		// Token: 0x04000F0F RID: 3855
		private InputBankTest ownerInputBank;

		// Token: 0x04000F10 RID: 3856
		[Header("Movement Parameters")]
		public float smoothDampTime = 0.3f;

		// Token: 0x04000F11 RID: 3857
		private Vector3 velocity;

		// Token: 0x04000F12 RID: 3858
		private float maxSpeed;

		// Token: 0x04000F13 RID: 3859
		[Header("Attack Parameters")]
		public float fireFrequency = 5f;

		// Token: 0x04000F14 RID: 3860
		public float damageCoefficientInitial = 6f;

		// Token: 0x04000F15 RID: 3861
		public float damageCoefficientFinal = 6f;

		// Token: 0x04000F16 RID: 3862
		public float procCoefficient = 0.5f;

		// Token: 0x04000F17 RID: 3863
		public float force;

		// Token: 0x04000F18 RID: 3864
		[Header("Charge")]
		public GameObject chargeEffect;

		// Token: 0x04000F19 RID: 3865
		public float chargeDuration = 3f;

		// Token: 0x04000F1A RID: 3866
		public float chargeMaxVelocity = 20f;

		// Token: 0x04000F1B RID: 3867
		private Transform chargeEffectTransform;

		// Token: 0x04000F1C RID: 3868
		[Header("Fire")]
		public GameObject fireEffect;

		// Token: 0x04000F1D RID: 3869
		public float fireDuration = 6f;

		// Token: 0x04000F1E RID: 3870
		public float fireMaxVelocity = 1f;

		// Token: 0x04000F1F RID: 3871
		public GameObject tracerEffectPrefab;

		// Token: 0x04000F20 RID: 3872
		public GameObject hitEffectPrefab;

		// Token: 0x04000F21 RID: 3873
		[Header("Decay")]
		public float decayDuration = 1.5f;

		// Token: 0x04000F22 RID: 3874
		[Tooltip("The transform of the child laser pointer effect.")]
		[Header("Laser Pointer")]
		public Transform laserPointerEffectTransform;

		// Token: 0x04000F23 RID: 3875
		[Tooltip("The transform of the muzzle effect.")]
		public Transform muzzleEffectTransform;

		// Token: 0x04000F24 RID: 3876
		private Vector3 mostRecentPointerPosition;

		// Token: 0x04000F25 RID: 3877
		private Vector3 mostRecentPointerNormal;

		// Token: 0x04000F26 RID: 3878
		private Vector3 mostRecentMuzzlePosition;

		// Token: 0x020002B3 RID: 691
		private abstract class OrbitalLaserBaseState : BaseState
		{
			// Token: 0x06000FA7 RID: 4007 RVA: 0x00044B21 File Offset: 0x00042D21
			public override void OnEnter()
			{
				base.OnEnter();
				this.controller = base.GetComponent<OrbitalLaserController>();
			}

			// Token: 0x04000F27 RID: 3879
			protected OrbitalLaserController controller;
		}

		// Token: 0x020002B4 RID: 692
		private class OrbitalLaserChargeState : OrbitalLaserController.OrbitalLaserBaseState
		{
			// Token: 0x06000FA9 RID: 4009 RVA: 0x00044B3D File Offset: 0x00042D3D
			public override void OnEnter()
			{
				base.OnEnter();
				this.controller.chargeEffect.SetActive(true);
				this.controller.maxSpeed = this.controller.chargeMaxVelocity;
			}

			// Token: 0x06000FAA RID: 4010 RVA: 0x00044B6C File Offset: 0x00042D6C
			public override void OnExit()
			{
				this.controller.chargeEffect.SetActive(false);
				base.OnExit();
			}

			// Token: 0x06000FAB RID: 4011 RVA: 0x00044B85 File Offset: 0x00042D85
			public override void FixedUpdate()
			{
				base.FixedUpdate();
				if (NetworkServer.active && base.fixedAge >= this.controller.chargeDuration)
				{
					this.outer.SetNextState(new OrbitalLaserController.OrbitalLaserFireState());
					return;
				}
			}
		}

		// Token: 0x020002B5 RID: 693
		private class OrbitalLaserFireState : OrbitalLaserController.OrbitalLaserBaseState
		{
			// Token: 0x06000FAD RID: 4013 RVA: 0x00044BC0 File Offset: 0x00042DC0
			public override void OnEnter()
			{
				base.OnEnter();
				this.controller.fireEffect.SetActive(true);
				this.controller.maxSpeed = this.controller.fireMaxVelocity;
			}

			// Token: 0x06000FAE RID: 4014 RVA: 0x00044BEF File Offset: 0x00042DEF
			public override void OnExit()
			{
				this.controller.fireEffect.SetActive(false);
				base.OnExit();
			}

			// Token: 0x06000FAF RID: 4015 RVA: 0x00044C08 File Offset: 0x00042E08
			public override void FixedUpdate()
			{
				base.FixedUpdate();
				if (NetworkServer.active)
				{
					if (base.fixedAge >= this.controller.fireDuration || !this.controller.ownerBody)
					{
						this.outer.SetNextState(new OrbitalLaserController.OrbitalLaserDecayState());
						return;
					}
					this.bulletAttackTimer -= Time.fixedDeltaTime;
					if (this.controller.ownerBody && this.bulletAttackTimer < 0f)
					{
						this.bulletAttackTimer += 1f / this.controller.fireFrequency;
						new BulletAttack
						{
							owner = this.controller.ownerBody.gameObject,
							weapon = base.gameObject,
							origin = base.transform.position + Vector3.up * 600f,
							maxDistance = 1200f,
							aimVector = Vector3.down,
							minSpread = 0f,
							maxSpread = 0f,
							damage = Mathf.Lerp(this.controller.damageCoefficientInitial, this.controller.damageCoefficientFinal, base.fixedAge / this.controller.fireDuration) * this.controller.ownerBody.damage / this.controller.fireFrequency,
							force = this.controller.force,
							tracerEffectPrefab = this.controller.tracerEffectPrefab,
							muzzleName = "",
							hitEffectPrefab = this.controller.hitEffectPrefab,
							isCrit = Util.CheckRoll(this.controller.ownerBody.crit, this.controller.ownerBody.master),
							stopperMask = LayerIndex.world.mask,
							damageColorIndex = DamageColorIndex.Item,
							procCoefficient = this.controller.procCoefficient / this.controller.fireFrequency,
							radius = 2f
						}.Fire();
					}
				}
			}

			// Token: 0x04000F28 RID: 3880
			private float bulletAttackTimer;
		}

		// Token: 0x020002B6 RID: 694
		private class OrbitalLaserDecayState : OrbitalLaserController.OrbitalLaserBaseState
		{
			// Token: 0x06000FB1 RID: 4017 RVA: 0x00044E28 File Offset: 0x00043028
			public override void OnEnter()
			{
				base.OnEnter();
				this.controller.maxSpeed = 0f;
			}

			// Token: 0x06000FB2 RID: 4018 RVA: 0x00044E40 File Offset: 0x00043040
			public override void FixedUpdate()
			{
				base.FixedUpdate();
				if (NetworkServer.active && base.fixedAge >= this.controller.decayDuration)
				{
					EntityState.Destroy(base.gameObject);
				}
			}
		}
	}
}
