using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000176 RID: 374
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(Rigidbody))]
	public class ChainController : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06000709 RID: 1801 RVA: 0x0001DBDC File Offset: 0x0001BDDC
		private void Start()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.transform = base.transform;
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.teamFilter = base.GetComponent<TeamFilter>();
			this.rigidbody.velocity = this.transform.forward * this.maxVelocity;
			this.pastTargetList = new List<Transform>();
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x0001DC60 File Offset: 0x0001BE60
		private void FixedUpdate()
		{
			if (!this.currentTarget)
			{
				this.stopwatch += Time.fixedDeltaTime;
				if (this.stopwatch > this.lifeTime)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				if (this.assignFirstTarget || (this.bounceCount > 0 && this.searchAfterFirstFailure))
				{
					this.currentTarget = this.FindTarget();
					if (!this.currentTarget && this.destroyOnFailure)
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
					else if (this.bounceCount == 0 && this.ignoreFirstTarget)
					{
						this.bounceCount++;
						this.pastTargetList.Add(this.currentTarget);
						this.currentTarget = null;
					}
				}
				this.rigidbody.velocity = this.transform.forward * this.maxVelocity;
				return;
			}
			this.stopwatch = 0f;
			Vector3 vector = this.currentTarget.transform.position - this.rigidbody.position;
			if (vector != Vector3.zero)
			{
				this.transform.rotation = Util.QuaternionSafeLookRotation(vector);
			}
			this.rigidbody.velocity = this.transform.forward * Mathf.Min(this.maxVelocity, Vector3.Magnitude(vector) / Time.fixedDeltaTime);
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x0001DDC4 File Offset: 0x0001BFC4
		private Transform FindTarget()
		{
			TeamIndex teamIndex = TeamIndex.Monster;
			TeamIndex teamIndex2 = this.teamFilter.teamIndex;
			if (teamIndex2 != TeamIndex.Player)
			{
				if (teamIndex2 == TeamIndex.Monster)
				{
					teamIndex = TeamIndex.Player;
				}
			}
			else
			{
				teamIndex = TeamIndex.Monster;
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
			float num = 99999f;
			Transform result = null;
			Vector3 position = this.transform.position;
			for (int i = 0; i < teamMembers.Count; i++)
			{
				Transform transform = teamMembers[i].transform;
				if (!this.pastTargetList.Contains(transform) || (this.canBounceToSameTarget && transform != this.lastTarget))
				{
					float num2 = Vector3.SqrMagnitude(transform.position - position);
					if (num2 < num && (!this.smartSeeking || !Physics.Raycast(position, transform.position - position, Mathf.Sqrt(num2), LayerIndex.world.mask)) && num2 <= this.maxChainDistance * this.maxChainDistance)
					{
						num = num2;
						result = transform;
					}
				}
			}
			return result;
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0001DEC8 File Offset: 0x0001C0C8
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			if (collider)
			{
				DamageInfo damageInfo = new DamageInfo();
				if (this.projectileDamage)
				{
					damageInfo.damage = this.projectileDamage.damage;
					damageInfo.crit = this.projectileDamage.crit;
					damageInfo.attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null);
					damageInfo.inflictor = base.gameObject;
					damageInfo.position = impactInfo.estimatedPointOfImpact;
					damageInfo.force = this.projectileDamage.force * this.transform.forward;
					damageInfo.procChainMask = this.projectileController.procChainMask;
					damageInfo.procCoefficient = this.projectileController.procCoefficient;
					damageInfo.damageColorIndex = this.projectileDamage.damageColorIndex;
				}
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						Transform transform = healthComponent.gameObject.transform;
						if (healthComponent.gameObject == this.projectileController.owner)
						{
							return;
						}
						if ((!this.pastTargetList.Contains(transform) || (this.canBounceToSameTarget && transform != this.lastTarget)) && (transform == this.currentTarget || this.canHitNonTarget || !this.currentTarget))
						{
							this.pastTargetList.Add(transform);
							if (this.currentTarget)
							{
								this.lastTarget = this.currentTarget;
							}
							else
							{
								this.lastTarget = transform;
							}
							this.currentTarget = this.FindTarget();
							this.bounceCount++;
							healthComponent.TakeDamage(damageInfo);
							GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
							GlobalEventManager.instance.OnHitEnemy(damageInfo, component.healthComponent.gameObject);
							if (this.projectileDamage)
							{
								this.projectileDamage.damage *= this.damageMultiplier;
							}
							if (this.impactEffect)
							{
								EffectManager.SimpleImpactEffect(this.impactEffect, this.transform.position, -this.transform.forward, !this.projectileController.isPrediction);
							}
							if (this.bounceCount >= this.maxBounceCount)
							{
								this.alive = false;
							}
						}
					}
				}
				else if (this.destroyOnWorldIfNoTarget && this.currentTarget == null)
				{
					damageInfo.position = this.transform.position;
					GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
					if (this.impactEffect)
					{
						EffectManager.SimpleImpactEffect(this.impactEffect, this.transform.position, -this.transform.forward, !this.projectileController.isPrediction);
					}
					this.alive = false;
				}
			}
			if (!this.alive)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04000782 RID: 1922
		private new Transform transform;

		// Token: 0x04000783 RID: 1923
		private Rigidbody rigidbody;

		// Token: 0x04000784 RID: 1924
		[HideInInspector]
		public Transform currentTarget;

		// Token: 0x04000785 RID: 1925
		private Transform lastTarget;

		// Token: 0x04000786 RID: 1926
		private TeamFilter teamFilter;

		// Token: 0x04000787 RID: 1927
		private ProjectileController projectileController;

		// Token: 0x04000788 RID: 1928
		private ProjectileDamage projectileDamage;

		// Token: 0x04000789 RID: 1929
		private bool alive = true;

		// Token: 0x0400078A RID: 1930
		private int bounceCount;

		// Token: 0x0400078B RID: 1931
		[HideInInspector]
		public List<Transform> pastTargetList;

		// Token: 0x0400078C RID: 1932
		public GameObject impactEffect;

		// Token: 0x0400078D RID: 1933
		public float maxVelocity;

		// Token: 0x0400078E RID: 1934
		public int maxBounceCount = 3;

		// Token: 0x0400078F RID: 1935
		public float maxChainDistance = 10f;

		// Token: 0x04000790 RID: 1936
		public float lifeTime = 5f;

		// Token: 0x04000791 RID: 1937
		private float stopwatch;

		// Token: 0x04000792 RID: 1938
		[Tooltip("Multiplier for damage on every chain. >1 for increasing damage, <1 for decreasing damage.")]
		public float damageMultiplier = 1f;

		// Token: 0x04000793 RID: 1939
		[Tooltip("Whether or not the projectile will automatically detect its first target or fly dumb until the first hit.")]
		public bool assignFirstTarget;

		// Token: 0x04000794 RID: 1940
		[Tooltip("Whether or not the projectile will automatically ignore the first target, preventing it from getting hit.")]
		public bool ignoreFirstTarget;

		// Token: 0x04000795 RID: 1941
		[Tooltip("Whether or not the projectile will destroy itself on colliding with terrain.")]
		public bool destroyOnWorldIfNoTarget;

		// Token: 0x04000796 RID: 1942
		[Tooltip("Whether or not the projectile can bounce between the same targets.")]
		public bool canBounceToSameTarget;

		// Token: 0x04000797 RID: 1943
		[Tooltip("Whether or not the projectile can hit targets accidentally on the way.")]
		public bool canHitNonTarget;

		// Token: 0x04000798 RID: 1944
		[Tooltip("Whether or not the projectile will raycast to make sure it can hit its target. Can be expensive.")]
		public bool smartSeeking;

		// Token: 0x04000799 RID: 1945
		[Tooltip("Whether or not the projectile will continue to search for targets after its first failure. Can be expensive, especially with smart seeking.")]
		public bool searchAfterFirstFailure;

		// Token: 0x0400079A RID: 1946
		[Tooltip("Whether or not the projectile should destruct on failure.")]
		public bool destroyOnFailure;
	}
}
