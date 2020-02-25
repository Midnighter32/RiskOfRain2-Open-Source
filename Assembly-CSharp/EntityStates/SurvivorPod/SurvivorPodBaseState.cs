using System;
using RoR2;

namespace EntityStates.SurvivorPod
{
	// Token: 0x0200077F RID: 1919
	public abstract class SurvivorPodBaseState : EntityState
	{
		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06002C1D RID: 11293 RVA: 0x000BA524 File Offset: 0x000B8724
		// (set) Token: 0x06002C1E RID: 11294 RVA: 0x000BA52C File Offset: 0x000B872C
		private protected SurvivorPodController survivorPodController { protected get; private set; }

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06002C1F RID: 11295 RVA: 0x000BA535 File Offset: 0x000B8735
		// (set) Token: 0x06002C20 RID: 11296 RVA: 0x000BA53D File Offset: 0x000B873D
		private protected VehicleSeat vehicleSeat { protected get; private set; }

		// Token: 0x06002C21 RID: 11297 RVA: 0x000BA548 File Offset: 0x000B8748
		public override void OnEnter()
		{
			base.OnEnter();
			this.survivorPodController = base.GetComponent<SurvivorPodController>();
			SurvivorPodController survivorPodController = this.survivorPodController;
			this.vehicleSeat = ((survivorPodController != null) ? survivorPodController.vehicleSeat : null);
			if (!this.survivorPodController && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}
	}
}
