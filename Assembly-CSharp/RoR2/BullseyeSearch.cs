using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200023C RID: 572
	public class BullseyeSearch
	{
		// Token: 0x170001A3 RID: 419
		// (set) Token: 0x06000CAF RID: 3247 RVA: 0x00039301 File Offset: 0x00037501
		public float minAngleFilter
		{
			set
			{
				this.maxThetaDot = Mathf.Cos(Mathf.Clamp(value, 0f, 180f) * 0.017453292f);
			}
		}

		// Token: 0x170001A4 RID: 420
		// (set) Token: 0x06000CB0 RID: 3248 RVA: 0x00039324 File Offset: 0x00037524
		public float maxAngleFilter
		{
			set
			{
				this.minThetaDot = Mathf.Cos(Mathf.Clamp(value, 0f, 180f) * 0.017453292f);
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000CB1 RID: 3249 RVA: 0x00039347 File Offset: 0x00037547
		private bool filterByDistance
		{
			get
			{
				return this.minDistanceFilter > 0f || this.maxDistanceFilter < float.PositiveInfinity;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000CB2 RID: 3250 RVA: 0x00039365 File Offset: 0x00037565
		private bool filterByAngle
		{
			get
			{
				return this.minThetaDot > -1f || this.maxThetaDot < 1f;
			}
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x00039384 File Offset: 0x00037584
		private Func<HurtBox, BullseyeSearch.CandidateInfo> GetSelector()
		{
			bool getDot = this.filterByAngle;
			bool getDistanceSqr = this.filterByDistance;
			getDistanceSqr |= (this.sortMode == BullseyeSearch.SortMode.Distance || this.sortMode == BullseyeSearch.SortMode.DistanceAndAngle);
			getDot |= (this.sortMode == BullseyeSearch.SortMode.Angle || this.sortMode == BullseyeSearch.SortMode.DistanceAndAngle);
			bool getDifference = getDot | getDistanceSqr;
			bool getPosition = (getDot | getDistanceSqr) || this.filterByLoS;
			return delegate(HurtBox hurtBox)
			{
				BullseyeSearch.CandidateInfo candidateInfo = new BullseyeSearch.CandidateInfo
				{
					hurtBox = hurtBox
				};
				if (getPosition)
				{
					candidateInfo.position = hurtBox.transform.position;
				}
				Vector3 vector = default(Vector3);
				if (getDifference)
				{
					vector = candidateInfo.position - this.searchOrigin;
				}
				if (getDot)
				{
					candidateInfo.dot = Vector3.Dot(this.searchDirection, vector.normalized);
				}
				if (getDistanceSqr)
				{
					candidateInfo.distanceSqr = vector.sqrMagnitude;
				}
				return candidateInfo;
			};
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x00039438 File Offset: 0x00037638
		public void RefreshCandidates()
		{
			Func<HurtBox, BullseyeSearch.CandidateInfo> selector = this.GetSelector();
			this.candidatesEnumerable = (from hurtBox in HurtBox.readOnlyBullseyesList
			where this.teamMaskFilter.HasTeam(hurtBox.teamIndex)
			select hurtBox).Select(selector);
			if (this.filterByAngle)
			{
				this.candidatesEnumerable = this.candidatesEnumerable.Where(new Func<BullseyeSearch.CandidateInfo, bool>(this.<RefreshCandidates>g__DotOkay|25_1));
			}
			if (this.filterByDistance)
			{
				BullseyeSearch.<>c__DisplayClass25_0 CS$<>8__locals1 = new BullseyeSearch.<>c__DisplayClass25_0();
				CS$<>8__locals1.minDistanceSqr = this.minDistanceFilter * this.minDistanceFilter;
				CS$<>8__locals1.maxDistanceSqr = this.maxDistanceFilter * this.maxDistanceFilter;
				this.candidatesEnumerable = this.candidatesEnumerable.Where(new Func<BullseyeSearch.CandidateInfo, bool>(CS$<>8__locals1.<RefreshCandidates>g__DistanceOkay|2));
			}
			if (this.filterByDistinctEntity)
			{
				this.candidatesEnumerable = this.candidatesEnumerable.Distinct(default(BullseyeSearch.CandidateInfo.EntityEqualityComparer));
			}
			Func<BullseyeSearch.CandidateInfo, float> sorter = this.GetSorter();
			this.candidatesEnumerable = this.candidatesEnumerable.OrderBy(sorter);
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x00039524 File Offset: 0x00037724
		private Func<BullseyeSearch.CandidateInfo, float> GetSorter()
		{
			switch (this.sortMode)
			{
			case BullseyeSearch.SortMode.Distance:
				return (BullseyeSearch.CandidateInfo candidateInfo) => candidateInfo.distanceSqr;
			case BullseyeSearch.SortMode.Angle:
				return (BullseyeSearch.CandidateInfo candidateInfo) => -candidateInfo.dot;
			case BullseyeSearch.SortMode.DistanceAndAngle:
				return (BullseyeSearch.CandidateInfo candidateInfo) => -candidateInfo.dot * candidateInfo.distanceSqr;
			default:
				return null;
			}
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x000395B0 File Offset: 0x000377B0
		public void FilterCandidatesByHealthFraction(float minHealthFraction = 0f, float maxHealthFraction = 1f)
		{
			if (minHealthFraction > 0f)
			{
				if (maxHealthFraction < 1f)
				{
					this.candidatesEnumerable = this.candidatesEnumerable.Where(delegate(BullseyeSearch.CandidateInfo v)
					{
						float combinedHealthFraction = v.hurtBox.healthComponent.combinedHealthFraction;
						return combinedHealthFraction >= minHealthFraction && combinedHealthFraction <= maxHealthFraction;
					});
					return;
				}
				this.candidatesEnumerable = from v in this.candidatesEnumerable
				where v.hurtBox.healthComponent.combinedHealthFraction >= minHealthFraction
				select v;
				return;
			}
			else
			{
				if (maxHealthFraction < 1f)
				{
					this.candidatesEnumerable = from v in this.candidatesEnumerable
					where v.hurtBox.healthComponent.combinedHealthFraction <= maxHealthFraction
					select v;
					return;
				}
				return;
			}
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x00039654 File Offset: 0x00037854
		public void FilterOutGameObject(GameObject gameObject)
		{
			this.candidatesEnumerable = from v in this.candidatesEnumerable
			where v.hurtBox.healthComponent.gameObject != gameObject
			select v;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x0003968C File Offset: 0x0003788C
		public IEnumerable<HurtBox> GetResults()
		{
			IEnumerable<BullseyeSearch.CandidateInfo> source = this.candidatesEnumerable;
			if (this.filterByLoS)
			{
				source = from candidateInfo in source
				where this.CheckLoS(candidateInfo.position)
				select candidateInfo;
			}
			if (this.viewer)
			{
				source = from candidateInfo in source
				where this.CheckVisible(candidateInfo.hurtBox.healthComponent.gameObject)
				select candidateInfo;
			}
			return from candidateInfo in source
			select candidateInfo.hurtBox;
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x00039700 File Offset: 0x00037900
		private bool CheckLoS(Vector3 targetPosition)
		{
			Vector3 direction = targetPosition - this.searchOrigin;
			RaycastHit raycastHit;
			return !Physics.Raycast(this.searchOrigin, direction, out raycastHit, direction.magnitude, LayerIndex.world.mask, this.queryTriggerInteraction);
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x0003974C File Offset: 0x0003794C
		private bool CheckVisible(GameObject gameObject)
		{
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			return !component || component.GetVisibilityLevel(this.viewer) >= VisibilityLevel.Revealed;
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x000397DC File Offset: 0x000379DC
		[CompilerGenerated]
		private bool <RefreshCandidates>g__DotOkay|25_1(BullseyeSearch.CandidateInfo candidateInfo)
		{
			return this.minThetaDot <= candidateInfo.dot && candidateInfo.dot <= this.maxThetaDot;
		}

		// Token: 0x04000CC8 RID: 3272
		public CharacterBody viewer;

		// Token: 0x04000CC9 RID: 3273
		public Vector3 searchOrigin;

		// Token: 0x04000CCA RID: 3274
		public Vector3 searchDirection;

		// Token: 0x04000CCB RID: 3275
		private float minThetaDot = -1f;

		// Token: 0x04000CCC RID: 3276
		private float maxThetaDot = 1f;

		// Token: 0x04000CCD RID: 3277
		public float minDistanceFilter;

		// Token: 0x04000CCE RID: 3278
		public float maxDistanceFilter = float.PositiveInfinity;

		// Token: 0x04000CCF RID: 3279
		public TeamMask teamMaskFilter = TeamMask.allButNeutral;

		// Token: 0x04000CD0 RID: 3280
		public bool filterByLoS = true;

		// Token: 0x04000CD1 RID: 3281
		public bool filterByDistinctEntity;

		// Token: 0x04000CD2 RID: 3282
		public QueryTriggerInteraction queryTriggerInteraction;

		// Token: 0x04000CD3 RID: 3283
		public BullseyeSearch.SortMode sortMode = BullseyeSearch.SortMode.Distance;

		// Token: 0x04000CD4 RID: 3284
		private IEnumerable<BullseyeSearch.CandidateInfo> candidatesEnumerable;

		// Token: 0x0200023D RID: 573
		private struct CandidateInfo
		{
			// Token: 0x04000CD5 RID: 3285
			public HurtBox hurtBox;

			// Token: 0x04000CD6 RID: 3286
			public Vector3 position;

			// Token: 0x04000CD7 RID: 3287
			public float dot;

			// Token: 0x04000CD8 RID: 3288
			public float distanceSqr;

			// Token: 0x0200023E RID: 574
			public struct EntityEqualityComparer : IEqualityComparer<BullseyeSearch.CandidateInfo>
			{
				// Token: 0x06000CC0 RID: 3264 RVA: 0x00039825 File Offset: 0x00037A25
				public bool Equals(BullseyeSearch.CandidateInfo a, BullseyeSearch.CandidateInfo b)
				{
					return a.hurtBox.healthComponent == b.hurtBox.healthComponent;
				}

				// Token: 0x06000CC1 RID: 3265 RVA: 0x0003983F File Offset: 0x00037A3F
				public int GetHashCode(BullseyeSearch.CandidateInfo obj)
				{
					return obj.hurtBox.healthComponent.GetHashCode();
				}
			}
		}

		// Token: 0x0200023F RID: 575
		public enum SortMode
		{
			// Token: 0x04000CDA RID: 3290
			None,
			// Token: 0x04000CDB RID: 3291
			Distance,
			// Token: 0x04000CDC RID: 3292
			Angle,
			// Token: 0x04000CDD RID: 3293
			DistanceAndAngle
		}

		// Token: 0x02000240 RID: 576
		// (Invoke) Token: 0x06000CC3 RID: 3267
		private delegate BullseyeSearch.CandidateInfo Selector(HurtBox hurtBox);
	}
}
