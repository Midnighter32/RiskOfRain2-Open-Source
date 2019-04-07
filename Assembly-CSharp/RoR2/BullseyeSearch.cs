using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200031C RID: 796
	public class BullseyeSearch
	{
		// Token: 0x1700016A RID: 362
		// (set) Token: 0x0600106A RID: 4202 RVA: 0x0005252F File Offset: 0x0005072F
		public float maxAngleFilter
		{
			set
			{
				if (value >= 180f)
				{
					this.minThetaDot = BullseyeSearch.fullVisionMinThetaDot;
					return;
				}
				this.minThetaDot = Mathf.Cos(value * 0.017453292f);
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x0600106B RID: 4203 RVA: 0x00052557 File Offset: 0x00050757
		private bool filterByDistance
		{
			get
			{
				return this.minDistanceFilter > 0f || this.maxDistanceFilter < float.PositiveInfinity;
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x0600106C RID: 4204 RVA: 0x00052575 File Offset: 0x00050775
		private bool filterByAngle
		{
			get
			{
				return this.minThetaDot > BullseyeSearch.fullVisionMinThetaDot;
			}
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x00052584 File Offset: 0x00050784
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

		// Token: 0x0600106E RID: 4206 RVA: 0x00052638 File Offset: 0x00050838
		public void RefreshCandidates()
		{
			Func<HurtBox, BullseyeSearch.CandidateInfo> selector = this.GetSelector();
			this.candidatesEnumerable = (from hurtBox in HurtBox.readOnlyBullseyesList
			where this.teamMaskFilter.HasTeam(hurtBox.teamIndex)
			select hurtBox).Select(selector);
			if (this.filterByAngle)
			{
				this.candidatesEnumerable = from candidateInfo in this.candidatesEnumerable
				where candidateInfo.dot >= this.minThetaDot
				select candidateInfo;
			}
			if (this.filterByDistance)
			{
				float minDistanceSqr = this.minDistanceFilter * this.minDistanceFilter;
				float maxDistanceSqr = this.maxDistanceFilter * this.maxDistanceFilter;
				this.candidatesEnumerable = from candidateInfo in this.candidatesEnumerable
				where candidateInfo.distanceSqr >= minDistanceSqr && candidateInfo.distanceSqr <= maxDistanceSqr
				select candidateInfo;
			}
			Func<BullseyeSearch.CandidateInfo, float> sorter = this.GetSorter();
			this.candidatesEnumerable = this.candidatesEnumerable.OrderBy(sorter);
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x00052700 File Offset: 0x00050900
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

		// Token: 0x06001070 RID: 4208 RVA: 0x0005278C File Offset: 0x0005098C
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

		// Token: 0x06001071 RID: 4209 RVA: 0x00052830 File Offset: 0x00050A30
		public void FilterOutGameObject(GameObject gameObject)
		{
			this.candidatesEnumerable = from v in this.candidatesEnumerable
			where v.hurtBox.healthComponent.gameObject != gameObject
			select v;
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00052868 File Offset: 0x00050A68
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
				where this.CheckVisisble(candidateInfo.hurtBox.healthComponent.gameObject)
				select candidateInfo;
			}
			return from candidateInfo in source
			select candidateInfo.hurtBox;
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x000528DC File Offset: 0x00050ADC
		private bool CheckLoS(Vector3 targetPosition)
		{
			Vector3 direction = targetPosition - this.searchOrigin;
			RaycastHit raycastHit;
			return !Physics.Raycast(this.searchOrigin, direction, out raycastHit, direction.magnitude, LayerIndex.world.mask, this.queryTriggerInteraction);
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x00052928 File Offset: 0x00050B28
		private bool CheckVisisble(GameObject gameObject)
		{
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			return !component || component.GetVisibilityLevel(this.viewer) >= VisibilityLevel.Revealed;
		}

		// Token: 0x0400148B RID: 5259
		public CharacterBody viewer;

		// Token: 0x0400148C RID: 5260
		public Vector3 searchOrigin;

		// Token: 0x0400148D RID: 5261
		public Vector3 searchDirection;

		// Token: 0x0400148E RID: 5262
		private static readonly float fullVisionMinThetaDot = Mathf.Cos(3.1415927f);

		// Token: 0x0400148F RID: 5263
		private float minThetaDot = -1f;

		// Token: 0x04001490 RID: 5264
		public float minDistanceFilter;

		// Token: 0x04001491 RID: 5265
		public float maxDistanceFilter = float.PositiveInfinity;

		// Token: 0x04001492 RID: 5266
		public TeamMask teamMaskFilter = TeamMask.allButNeutral;

		// Token: 0x04001493 RID: 5267
		public bool filterByLoS = true;

		// Token: 0x04001494 RID: 5268
		public QueryTriggerInteraction queryTriggerInteraction;

		// Token: 0x04001495 RID: 5269
		public BullseyeSearch.SortMode sortMode = BullseyeSearch.SortMode.Distance;

		// Token: 0x04001496 RID: 5270
		private IEnumerable<BullseyeSearch.CandidateInfo> candidatesEnumerable;

		// Token: 0x0200031D RID: 797
		private struct CandidateInfo
		{
			// Token: 0x04001497 RID: 5271
			public HurtBox hurtBox;

			// Token: 0x04001498 RID: 5272
			public Vector3 position;

			// Token: 0x04001499 RID: 5273
			public float dot;

			// Token: 0x0400149A RID: 5274
			public float distanceSqr;
		}

		// Token: 0x0200031E RID: 798
		public enum SortMode
		{
			// Token: 0x0400149C RID: 5276
			None,
			// Token: 0x0400149D RID: 5277
			Distance,
			// Token: 0x0400149E RID: 5278
			Angle,
			// Token: 0x0400149F RID: 5279
			DistanceAndAngle
		}

		// Token: 0x0200031F RID: 799
		// (Invoke) Token: 0x0600107C RID: 4220
		private delegate BullseyeSearch.CandidateInfo Selector(HurtBox hurtBox);
	}
}
