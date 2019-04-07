using System;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000318 RID: 792
	[RequireComponent(typeof(CharacterBody))]
	[RequireComponent(typeof(InputBankTest))]
	[RequireComponent(typeof(TeamComponent))]
	public class HuntressTracker : MonoBehaviour
	{
		// Token: 0x06001055 RID: 4181 RVA: 0x000521F5 File Offset: 0x000503F5
		private void Awake()
		{
			this.indicator = new Indicator(base.gameObject, Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x00052212 File Offset: 0x00050412
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.teamComponent = base.GetComponent<TeamComponent>();
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x00052238 File Offset: 0x00050438
		public HurtBox GetTrackingTarget()
		{
			return this.trackingTarget;
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x00052240 File Offset: 0x00050440
		private void OnEnable()
		{
			this.indicator.active = true;
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x0005224E File Offset: 0x0005044E
		private void OnDisable()
		{
			this.indicator.active = false;
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x0005225C File Offset: 0x0005045C
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

		// Token: 0x0600105B RID: 4187 RVA: 0x000522FC File Offset: 0x000504FC
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

		// Token: 0x04001471 RID: 5233
		public float maxTrackingDistance = 20f;

		// Token: 0x04001472 RID: 5234
		public float maxTrackingAngle = 20f;

		// Token: 0x04001473 RID: 5235
		public float trackerUpdateFrequency = 10f;

		// Token: 0x04001474 RID: 5236
		private HurtBox trackingTarget;

		// Token: 0x04001475 RID: 5237
		private CharacterBody characterBody;

		// Token: 0x04001476 RID: 5238
		private TeamComponent teamComponent;

		// Token: 0x04001477 RID: 5239
		private InputBankTest inputBank;

		// Token: 0x04001478 RID: 5240
		private float trackerUpdateStopwatch;

		// Token: 0x04001479 RID: 5241
		private Indicator indicator;

		// Token: 0x0400147A RID: 5242
		private readonly BullseyeSearch search = new BullseyeSearch();
	}
}
