using System;
using RoR2;

namespace EntityStates.Missions.Arena.NullWard
{
	// Token: 0x020007BB RID: 1979
	public class NullWardBaseState : EntityState
	{
		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06002D3B RID: 11579 RVA: 0x000197F3 File Offset: 0x000179F3
		protected ArenaMissionController arenaMissionController
		{
			get
			{
				return ArenaMissionController.instance;
			}
		}

		// Token: 0x06002D3C RID: 11580 RVA: 0x000BEFBC File Offset: 0x000BD1BC
		public override void OnEnter()
		{
			base.OnEnter();
			this.buffWard = base.GetComponent<BuffWard>();
			this.buffWard.enabled = true;
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
			this.childLocator = base.GetComponent<ChildLocator>();
			base.gameObject.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
		}

		// Token: 0x0400297B RID: 10619
		public static float wardRadiusOff;

		// Token: 0x0400297C RID: 10620
		public static float wardRadiusOn;

		// Token: 0x0400297D RID: 10621
		public static float wardWaitingRadius;

		// Token: 0x0400297E RID: 10622
		protected BuffWard buffWard;

		// Token: 0x0400297F RID: 10623
		protected PurchaseInteraction purchaseInteraction;

		// Token: 0x04002980 RID: 10624
		protected ChildLocator childLocator;
	}
}
