using System;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000238 RID: 568
	[RequireComponent(typeof(TeamComponent))]
	[RequireComponent(typeof(CharacterBody))]
	[RequireComponent(typeof(InputBankTest))]
	public class HuntressTracker : MonoBehaviour
	{
		// Token: 0x06000C97 RID: 3223 RVA: 0x00038FB1 File Offset: 0x000371B1
		private void Awake()
		{
			this.indicator = new Indicator(base.gameObject, Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x00038FCE File Offset: 0x000371CE
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.teamComponent = base.GetComponent<TeamComponent>();
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x00038FF4 File Offset: 0x000371F4
		public HurtBox GetTrackingTarget()
		{
			return this.trackingTarget;
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x00038FFC File Offset: 0x000371FC
		private void OnEnable()
		{
			this.indicator.active = true;
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x0003900A File Offset: 0x0003720A
		private void OnDisable()
		{
			this.indicator.active = false;
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x00039018 File Offset: 0x00037218
		private void FixedUpdate()
		{
			this.trackerUpdateStopwatch += Time.fixedDeltaTime;
			if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
			{
				this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
				HurtBox hurtBox = this.trackingTarget;
				Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
				this.SearchForTarget(aimRay);
				this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
			}
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x000390B8 File Offset: 0x000372B8
		private void SearchForTarget(Ray aimRay)
		{
			this.search.teamMaskFilter = TeamMask.all;
			this.search.teamMaskFilter.RemoveTeam(this.teamComponent.teamIndex);
			this.search.filterByLoS = true;
			this.search.searchOrigin = aimRay.origin;
			this.search.searchDirection = aimRay.direction;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.maxTrackingDistance;
			this.search.maxAngleFilter = this.maxTrackingAngle;
			this.search.RefreshCandidates();
			this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
		}

		// Token: 0x04000CAF RID: 3247
		public float maxTrackingDistance = 20f;

		// Token: 0x04000CB0 RID: 3248
		public float maxTrackingAngle = 20f;

		// Token: 0x04000CB1 RID: 3249
		public float trackerUpdateFrequency = 10f;

		// Token: 0x04000CB2 RID: 3250
		private HurtBox trackingTarget;

		// Token: 0x04000CB3 RID: 3251
		private CharacterBody characterBody;

		// Token: 0x04000CB4 RID: 3252
		private TeamComponent teamComponent;

		// Token: 0x04000CB5 RID: 3253
		private InputBankTest inputBank;

		// Token: 0x04000CB6 RID: 3254
		private float trackerUpdateStopwatch;

		// Token: 0x04000CB7 RID: 3255
		private Indicator indicator;

		// Token: 0x04000CB8 RID: 3256
		private readonly BullseyeSearch search = new BullseyeSearch();
	}
}
