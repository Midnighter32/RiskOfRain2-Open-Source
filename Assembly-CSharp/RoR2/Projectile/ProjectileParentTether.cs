using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055C RID: 1372
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileParentTether : MonoBehaviour
	{
		// Token: 0x06001E9A RID: 7834 RVA: 0x0009054A File Offset: 0x0008E74A
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.attackTimer = 0f;
			this.UpdateTetherGraphic();
		}

		// Token: 0x06001E9B RID: 7835 RVA: 0x00090578 File Offset: 0x0008E778
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

		// Token: 0x06001E9C RID: 7836 RVA: 0x00090668 File Offset: 0x0008E868
		private float GetRayDistance()
		{
			if (this.projectileController.owner)
			{
				return (this.projectileController.owner.transform.position - base.transform.position).magnitude;
			}
			return 0f;
		}

		// Token: 0x06001E9D RID: 7837 RVA: 0x000906BC File Offset: 0x0008E8BC
		private Ray GetAimRay()
		{
			Ray result = default(Ray);
			result.origin = base.transform.position;
			result.direction = this.projectileController.owner.transform.position - result.origin;
			return result;
		}

		// Token: 0x06001E9E RID: 7838 RVA: 0x0009070C File Offset: 0x0008E90C
		private bool ShouldIFire()
		{
			return !this.stickOnImpact || this.stickOnImpact.stuck;
		}

		// Token: 0x06001E9F RID: 7839 RVA: 0x00090728 File Offset: 0x0008E928
		private void Update()
		{
			this.UpdateTetherGraphic();
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x00090730 File Offset: 0x0008E930
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

		// Token: 0x04002138 RID: 8504
		private ProjectileController projectileController;

		// Token: 0x04002139 RID: 8505
		private ProjectileDamage projectileDamage;

		// Token: 0x0400213A RID: 8506
		private TeamIndex myTeamIndex;

		// Token: 0x0400213B RID: 8507
		public float attackInterval = 1f;

		// Token: 0x0400213C RID: 8508
		public float maxTetherRange = 20f;

		// Token: 0x0400213D RID: 8509
		public float procCoefficient = 0.1f;

		// Token: 0x0400213E RID: 8510
		public float damageCoefficient = 1f;

		// Token: 0x0400213F RID: 8511
		public float raycastRadius;

		// Token: 0x04002140 RID: 8512
		public float lifetime;

		// Token: 0x04002141 RID: 8513
		public GameObject impactEffect;

		// Token: 0x04002142 RID: 8514
		public GameObject tetherEffectPrefab;

		// Token: 0x04002143 RID: 8515
		public ProjectileStickOnImpact stickOnImpact;

		// Token: 0x04002144 RID: 8516
		private GameObject tetherEffectInstance;

		// Token: 0x04002145 RID: 8517
		private GameObject tetherEffectInstanceEnd;

		// Token: 0x04002146 RID: 8518
		private float attackTimer;

		// Token: 0x04002147 RID: 8519
		private float lifetimeStopwatch;
	}
}
