using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2.Projectile
{
	// Token: 0x02000526 RID: 1318
	[RequireComponent(typeof(TeamFilter))]
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class ProjectileSphereTargetFinder : MonoBehaviour
	{
		// Token: 0x06001F23 RID: 7971 RVA: 0x000873FC File Offset: 0x000855FC
		private void Start()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.transform = base.transform;
			this.teamFilter = base.GetComponent<TeamFilter>();
			this.targetComponent = base.GetComponent<ProjectileTargetComponent>();
			this.sphereSearch = new SphereSearch();
			this.searchTimer = 0f;
		}

		// Token: 0x06001F24 RID: 7972 RVA: 0x00087454 File Offset: 0x00085654
		private void FixedUpdate()
		{
			this.searchTimer -= Time.fixedDeltaTime;
			if (this.searchTimer <= 0f)
			{
				this.searchTimer += this.targetSearchInterval;
				if (this.allowTargetLoss && this.targetComponent.target != null && this.lastFoundTransform == this.targetComponent.target && !this.PassesFilters(this.lastFoundHurtBox))
				{
					this.SetTarget(null);
				}
				if (!this.onlySearchIfNoTarget || this.targetComponent.target == null)
				{
					this.SearchForTarget();
				}
				this.hasTarget = (this.targetComponent.target != null);
				if (this.hadTargetLastUpdate != this.hasTarget)
				{
					if (this.hasTarget)
					{
						UnityEvent unityEvent = this.onNewTargetFound;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
					}
					else
					{
						UnityEvent unityEvent2 = this.onTargetLost;
						if (unityEvent2 != null)
						{
							unityEvent2.Invoke();
						}
					}
				}
				this.hadTargetLastUpdate = this.hasTarget;
			}
		}

		// Token: 0x06001F25 RID: 7973 RVA: 0x00087554 File Offset: 0x00085754
		private bool PassesFilters(HurtBox result)
		{
			CharacterBody body = result.healthComponent.body;
			return body && (!this.ignoreAir || !body.isFlying) && (!body.isFlying || float.IsInfinity(this.flierAltitudeTolerance) || this.flierAltitudeTolerance >= Mathf.Abs(result.transform.position.y - this.transform.position.y));
		}

		// Token: 0x06001F26 RID: 7974 RVA: 0x000875D0 File Offset: 0x000857D0
		private void SearchForTarget()
		{
			this.sphereSearch.origin = this.transform.position;
			this.sphereSearch.radius = this.lookRange;
			this.sphereSearch.mask = LayerIndex.entityPrecise.mask;
			this.sphereSearch.queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
			this.sphereSearch.RefreshCandidates();
			this.sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.AllExcept(this.teamFilter.teamIndex));
			this.sphereSearch.OrderCandidatesByDistance();
			this.sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
			this.sphereSearch.GetHurtBoxes(ProjectileSphereTargetFinder.foundHurtBoxes);
			HurtBox target = null;
			if (ProjectileSphereTargetFinder.foundHurtBoxes.Count > 0)
			{
				int i = 0;
				int count = ProjectileSphereTargetFinder.foundHurtBoxes.Count;
				while (i < count)
				{
					if (this.PassesFilters(ProjectileSphereTargetFinder.foundHurtBoxes[i]))
					{
						target = ProjectileSphereTargetFinder.foundHurtBoxes[i];
						break;
					}
					i++;
				}
				ProjectileSphereTargetFinder.foundHurtBoxes.Clear();
			}
			this.SetTarget(target);
		}

		// Token: 0x06001F27 RID: 7975 RVA: 0x000876D0 File Offset: 0x000858D0
		private void SetTarget(HurtBox hurtBox)
		{
			this.lastFoundHurtBox = hurtBox;
			this.lastFoundTransform = ((hurtBox != null) ? hurtBox.transform : null);
			this.targetComponent.target = this.lastFoundTransform;
		}

		// Token: 0x06001F28 RID: 7976 RVA: 0x000876FC File Offset: 0x000858FC
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Vector3 position = base.transform.position;
			Gizmos.DrawWireSphere(position, this.lookRange);
			if (!float.IsInfinity(this.flierAltitudeTolerance))
			{
				Gizmos.DrawWireCube(position, new Vector3(this.lookRange * 2f, this.flierAltitudeTolerance * 2f, this.lookRange * 2f));
			}
		}

		// Token: 0x04001CCE RID: 7374
		[Tooltip("How far ahead the projectile should look to find a target.")]
		public float lookRange;

		// Token: 0x04001CCF RID: 7375
		[Tooltip("How long before searching for a target.")]
		public float targetSearchInterval = 0.5f;

		// Token: 0x04001CD0 RID: 7376
		[Tooltip("Will not search for new targets once it has one.")]
		public bool onlySearchIfNoTarget;

		// Token: 0x04001CD1 RID: 7377
		[Tooltip("Allows the target to be lost if it's outside the acceptable range.")]
		public bool allowTargetLoss;

		// Token: 0x04001CD2 RID: 7378
		[Tooltip("If set, targets can only be found when there is a free line of sight.")]
		public bool testLoS;

		// Token: 0x04001CD3 RID: 7379
		[Tooltip("Whether or not airborne characters should be ignored.")]
		public bool ignoreAir;

		// Token: 0x04001CD4 RID: 7380
		[FormerlySerializedAs("altitudeTolerance")]
		[Tooltip("The difference in altitude at which a result will be ignored.")]
		public float flierAltitudeTolerance = float.PositiveInfinity;

		// Token: 0x04001CD5 RID: 7381
		public UnityEvent onNewTargetFound;

		// Token: 0x04001CD6 RID: 7382
		public UnityEvent onTargetLost;

		// Token: 0x04001CD7 RID: 7383
		private new Transform transform;

		// Token: 0x04001CD8 RID: 7384
		private TeamFilter teamFilter;

		// Token: 0x04001CD9 RID: 7385
		private ProjectileTargetComponent targetComponent;

		// Token: 0x04001CDA RID: 7386
		private float searchTimer;

		// Token: 0x04001CDB RID: 7387
		private SphereSearch sphereSearch;

		// Token: 0x04001CDC RID: 7388
		private bool hasTarget;

		// Token: 0x04001CDD RID: 7389
		private bool hadTargetLastUpdate;

		// Token: 0x04001CDE RID: 7390
		private HurtBox lastFoundHurtBox;

		// Token: 0x04001CDF RID: 7391
		private Transform lastFoundTransform;

		// Token: 0x04001CE0 RID: 7392
		private static readonly List<HurtBox> foundHurtBoxes = new List<HurtBox>();
	}
}
