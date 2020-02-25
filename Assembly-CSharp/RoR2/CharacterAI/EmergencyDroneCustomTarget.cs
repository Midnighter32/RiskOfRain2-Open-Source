using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.CharacterAI
{
	// Token: 0x02000571 RID: 1393
	[RequireComponent(typeof(BaseAI))]
	public class EmergencyDroneCustomTarget : MonoBehaviour
	{
		// Token: 0x06002144 RID: 8516 RVA: 0x0009002E File Offset: 0x0008E22E
		private void Awake()
		{
			this.ai = base.GetComponent<BaseAI>();
			if (NetworkServer.active)
			{
				this.search = new BullseyeSearch();
			}
		}

		// Token: 0x06002145 RID: 8517 RVA: 0x0009004E File Offset: 0x0008E24E
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x06002146 RID: 8518 RVA: 0x0009005D File Offset: 0x0008E25D
		private void FixedUpdateServer()
		{
			this.timer -= Time.fixedDeltaTime;
			if (this.timer <= 0f)
			{
				this.timer = this.searchInterval;
				this.DoSearch();
			}
		}

		// Token: 0x06002147 RID: 8519 RVA: 0x00090090 File Offset: 0x0008E290
		private void DoSearch()
		{
			if (this.ai.body)
			{
				Ray aimRay = this.ai.bodyInputBank.GetAimRay();
				this.search.viewer = this.ai.body;
				this.search.filterByDistinctEntity = true;
				this.search.filterByLoS = false;
				this.search.maxDistanceFilter = float.PositiveInfinity;
				this.search.minDistanceFilter = 0f;
				this.search.maxAngleFilter = 360f;
				this.search.searchDirection = aimRay.direction;
				this.search.searchOrigin = aimRay.origin;
				this.search.sortMode = BullseyeSearch.SortMode.Distance;
				this.search.queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
				TeamMask none = TeamMask.none;
				none.AddTeam(this.ai.master.teamIndex);
				this.search.teamMaskFilter = none;
				this.search.RefreshCandidates();
				this.search.FilterOutGameObject(this.ai.body.gameObject);
				BaseAI.Target customTarget = this.ai.customTarget;
				HurtBox hurtBox = this.search.GetResults().Where(new Func<HurtBox, bool>(this.TargetPassesFilters)).FirstOrDefault<HurtBox>();
				customTarget.gameObject = ((hurtBox != null) ? hurtBox.healthComponent.gameObject : null);
			}
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x000901EE File Offset: 0x0008E3EE
		private bool TargetPassesFilters(HurtBox hurtBox)
		{
			return EmergencyDroneCustomTarget.IsHurt(hurtBox) && !HealBeamController.HealBeamAlreadyExists(this.ai.body.gameObject, hurtBox.healthComponent);
		}

		// Token: 0x06002149 RID: 8521 RVA: 0x00090218 File Offset: 0x0008E418
		private static bool IsHurt(HurtBox hurtBox)
		{
			return hurtBox.healthComponent.alive && hurtBox.healthComponent.health < hurtBox.healthComponent.fullHealth;
		}

		// Token: 0x04001E91 RID: 7825
		private BaseAI ai;

		// Token: 0x04001E92 RID: 7826
		private BullseyeSearch search;

		// Token: 0x04001E93 RID: 7827
		public float searchInterval;

		// Token: 0x04001E94 RID: 7828
		private float timer;
	}
}
