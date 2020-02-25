using System;

namespace EntityStates.Missions.Arena.NullWard
{
	// Token: 0x020007BC RID: 1980
	public class Off : NullWardBaseState
	{
		// Token: 0x06002D3E RID: 11582 RVA: 0x000BF010 File Offset: 0x000BD210
		public override void OnEnter()
		{
			base.OnEnter();
			this.buffWard.Networkradius = NullWardBaseState.wardRadiusOff;
			this.purchaseInteraction.SetAvailable(false);
			this.buffWard.enabled = false;
		}
	}
}
