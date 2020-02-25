using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2.Projectile
{
	// Token: 0x02000502 RID: 1282
	[RequireComponent(typeof(TeamFilter))]
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class ProjectileDirectionalTargetFinder : MonoBehaviour
	{
		// Token: 0x06001E6F RID: 7791 RVA: 0x000832D8 File Offset: 0x000814D8
		private void Start()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.bullseyeSearch = new BullseyeSearch();
			this.teamFilter = base.GetComponent<TeamFilter>();
			this.targetComponent = base.GetComponent<ProjectileTargetComponent>();
			this.transform = base.transform;
			this.searchTimer = 0f;
		}

		// Token: 0x06001E70 RID: 7792 RVA: 0x00083330 File Offset: 0x00081530
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

		// Token: 0x06001E71 RID: 7793 RVA: 0x00083430 File Offset: 0x00081630
		private bool PassesFilters(HurtBox result)
		{
			CharacterBody body = result.healthComponent.body;
			return body && (!this.ignoreAir || !body.isFlying) && (!body.isFlying || float.IsInfinity(this.flierAltitudeTolerance) || this.flierAltitudeTolerance >= Mathf.Abs(result.transform.position.y - this.transform.position.y));
		}

		// Token: 0x06001E72 RID: 7794 RVA: 0x000834AC File Offset: 0x000816AC
		private void SearchForTarget()
		{
			this.bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			this.bullseyeSearch.teamMaskFilter.RemoveTeam(this.teamFilter.teamIndex);
			this.bullseyeSearch.filterByLoS = this.testLoS;
			this.bullseyeSearch.searchOrigin = this.transform.position;
			this.bullseyeSearch.searchDirection = this.transform.forward;
			this.bullseyeSearch.maxDistanceFilter = this.lookRange;
			this.bullseyeSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
			this.bullseyeSearch.maxAngleFilter = this.lookCone;
			this.bullseyeSearch.RefreshCandidates();
			IEnumerable<HurtBox> source = this.bullseyeSearch.GetResults().Where(new Func<HurtBox, bool>(this.PassesFilters));
			this.SetTarget(source.FirstOrDefault<HurtBox>());
		}

		// Token: 0x06001E73 RID: 7795 RVA: 0x00083583 File Offset: 0x00081783
		private void SetTarget(HurtBox hurtBox)
		{
			this.lastFoundHurtBox = hurtBox;
			this.lastFoundTransform = ((hurtBox != null) ? hurtBox.transform : null);
			this.targetComponent.target = this.lastFoundTransform;
		}

		// Token: 0x06001E74 RID: 7796 RVA: 0x000835B0 File Offset: 0x000817B0
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Gizmos.DrawWireSphere(position, this.lookRange);
			Gizmos.DrawRay(position, transform.forward * this.lookRange);
			Gizmos.DrawFrustum(position, this.lookCone * 2f, this.lookRange, 0f, 1f);
			if (!float.IsInfinity(this.flierAltitudeTolerance))
			{
				Gizmos.DrawWireCube(position, new Vector3(this.lookRange * 2f, this.flierAltitudeTolerance * 2f, this.lookRange * 2f));
			}
		}

		// Token: 0x04001BB5 RID: 7093
		[Tooltip("How far ahead the projectile should look to find a target.")]
		public float lookRange;

		// Token: 0x04001BB6 RID: 7094
		[Tooltip("How wide the cone of vision for this projectile is in degrees. Limit is 180.")]
		[Range(0f, 180f)]
		public float lookCone;

		// Token: 0x04001BB7 RID: 7095
		[Tooltip("How long before searching for a target.")]
		public float targetSearchInterval = 0.5f;

		// Token: 0x04001BB8 RID: 7096
		[Tooltip("Will not search for new targets once it has one.")]
		public bool onlySearchIfNoTarget;

		// Token: 0x04001BB9 RID: 7097
		[Tooltip("Allows the target to be lost if it's outside the acceptable range.")]
		public bool allowTargetLoss;

		// Token: 0x04001BBA RID: 7098
		[Tooltip("If set, targets can only be found when there is a free line of sight.")]
		public bool testLoS;

		// Token: 0x04001BBB RID: 7099
		[Tooltip("Whether or not airborne characters should be ignored.")]
		public bool ignoreAir;

		// Token: 0x04001BBC RID: 7100
		[FormerlySerializedAs("altitudeTolerance")]
		[Tooltip("The difference in altitude at which a result will be ignored.")]
		public float flierAltitudeTolerance = float.PositiveInfinity;

		// Token: 0x04001BBD RID: 7101
		public UnityEvent onNewTargetFound;

		// Token: 0x04001BBE RID: 7102
		[FormerlySerializedAs("ontargetLost")]
		public UnityEvent onTargetLost;

		// Token: 0x04001BBF RID: 7103
		private new Transform transform;

		// Token: 0x04001BC0 RID: 7104
		private TeamFilter teamFilter;

		// Token: 0x04001BC1 RID: 7105
		private ProjectileTargetComponent targetComponent;

		// Token: 0x04001BC2 RID: 7106
		private float searchTimer;

		// Token: 0x04001BC3 RID: 7107
		private bool hasTarget;

		// Token: 0x04001BC4 RID: 7108
		private bool hadTargetLastUpdate;

		// Token: 0x04001BC5 RID: 7109
		private BullseyeSearch bullseyeSearch;

		// Token: 0x04001BC6 RID: 7110
		private HurtBox lastFoundHurtBox;

		// Token: 0x04001BC7 RID: 7111
		private Transform lastFoundTransform;
	}
}
