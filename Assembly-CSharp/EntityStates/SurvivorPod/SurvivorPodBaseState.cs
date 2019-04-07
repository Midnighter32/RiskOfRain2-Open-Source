using System;
using RoR2;

namespace EntityStates.SurvivorPod
{
	// Token: 0x020000F2 RID: 242
	public abstract class SurvivorPodBaseState : EntityState
	{
		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0001350A File Offset: 0x0001170A
		// (set) Token: 0x060004A2 RID: 1186 RVA: 0x00013512 File Offset: 0x00011712
		private protected SurvivorPodController survivorPodController { protected get; private set; }

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001351B File Offset: 0x0001171B
		public override void OnEnter()
		{
			base.OnEnter();
			this.survivorPodController = base.GetComponent<SurvivorPodController>();
			if (!this.survivorPodController && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}
	}
}
