using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000522 RID: 1314
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileParentTether : MonoBehaviour
	{
		// Token: 0x06001F0B RID: 7947 RVA: 0x0008691D File Offset: 0x00084B1D
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.attackTimer = 0f;
			this.UpdateTetherGraphic();
		}

		// Token: 0x06001F0C RID: 7948 RVA: 0x00086948 File Offset: 0x00084B48
		private void UpdateTetherGraphic()
		{
			if (this.ShouldIFire())
			{
				if (this.tetherEffectPrefab && !this.tetherEffectInstance)
				{
					this.tetherEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.tetherEffectPrefab, base.transform.position, base.transform.rotation);
					this.tetherEffectInstance.transform.parent = base.transform;
					ChildLocator component = this.tetherEffectInstance.GetComponent<ChildLocator>();
					this.tetherEffectInstanceEnd = component.FindChild("LaserEnd").gameObject;
				}
				if (this.tetherEffectInstance)
				{
					Ray aimRay = this.GetAimRay();
					this.tetherEffectInstance.transform.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
					this.tetherEffectInstanceEnd.transform.position = aimRay.origin + aimRay.direction * this.GetRayDistance();
				}
			}
		}

		// Token: 0x06001F0D RID: 7949 RVA: 0x00086A38 File Offset: 0x00084C38
		private float GetRayDistance()
		{
			if (this.projectileController.owner)
			{
				return (this.projectileController.owner.transform.position - base.transform.position).magnitude;
			}
			return 0f;
		}

		// Token: 0x06001F0E RID: 7950 RVA: 0x00086A8C File Offset: 0x00084C8C
		private Ray GetAimRay()
		{
			Ray result = default(Ray);
			result.origin = base.transform.position;
			result.direction = this.projectileController.owner.transform.position - result.origin;
			return result;
		}

		// Token: 0x06001F0F RID: 7951 RVA: 0x00086ADC File Offset: 0x00084CDC
		private bool ShouldIFire()
		{
			return !this.stickOnImpact || this.stickOnImpact.stuck;
		}

		// Token: 0x06001F10 RID: 7952 RVA: 0x00086AF8 File Offset: 0x00084CF8
		private void Update()
		{
			this.UpdateTetherGraphic();
		}

		// Token: 0x06001F11 RID: 7953 RVA: 0x00086B00 File Offset: 0x00084D00
		private void FixedUpdate()
		{
			if (this.ShouldIFire())
			{
				this.lifetimeStopwatch += Time.fixedDeltaTime;
			}
			if (this.lifetimeStopwatch > this.lifetime)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.projectileController.owner.transform && this.ShouldIFire())
			{
				this.myTeamIndex = (this.projectileController.teamFilter ? this.projectileController.teamFilter.teamIndex : TeamIndex.Neutral);
				this.attackTimer -= Time.fixedDeltaTime;
				if (this.attackTimer <= 0f)
				{
					Ray aimRay = this.GetAimRay();
					this.attackTimer = this.attackInterval;
					if (aimRay.direction.magnitude < this.maxTetherRange && NetworkServer.active)
					{
						new BulletAttack
						{
							owner = this.projectileController.owner,
							origin = aimRay.origin,
							aimVector = aimRay.direction,
							minSpread = 0f,
							damage = this.damageCoefficient * this.projectileDamage.damage,
							force = 0f,
							hitEffectPrefab = this.impactEffect,
							isCrit = this.projectileDamage.crit,
							radius = this.raycastRadius,
							falloffModel = BulletAttack.FalloffModel.None,
							stopperMask = 0,
							hitMask = LayerIndex.entityPrecise.mask,
							procCoefficient = this.procCoefficient,
							maxDistance = this.GetRayDistance()
						}.Fire();
					}
				}
			}
		}

		// Token: 0x04001C9D RID: 7325
		private ProjectileController projectileController;

		// Token: 0x04001C9E RID: 7326
		private ProjectileDamage projectileDamage;

		// Token: 0x04001C9F RID: 7327
		private TeamIndex myTeamIndex;

		// Token: 0x04001CA0 RID: 7328
		public float attackInterval = 1f;

		// Token: 0x04001CA1 RID: 7329
		public float maxTetherRange = 20f;

		// Token: 0x04001CA2 RID: 7330
		public float procCoefficient = 0.1f;

		// Token: 0x04001CA3 RID: 7331
		public float damageCoefficient = 1f;

		// Token: 0x04001CA4 RID: 7332
		public float raycastRadius;

		// Token: 0x04001CA5 RID: 7333
		public float lifetime;

		// Token: 0x04001CA6 RID: 7334
		public GameObject impactEffect;

		// Token: 0x04001CA7 RID: 7335
		public GameObject tetherEffectPrefab;

		// Token: 0x04001CA8 RID: 7336
		public ProjectileStickOnImpact stickOnImpact;

		// Token: 0x04001CA9 RID: 7337
		private GameObject tetherEffectInstance;

		// Token: 0x04001CAA RID: 7338
		private GameObject tetherEffectInstanceEnd;

		// Token: 0x04001CAB RID: 7339
		private float attackTimer;

		// Token: 0x04001CAC RID: 7340
		private float lifetimeStopwatch;
	}
}
