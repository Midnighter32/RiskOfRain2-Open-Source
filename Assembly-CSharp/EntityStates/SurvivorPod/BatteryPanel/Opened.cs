using System;

namespace EntityStates.SurvivorPod.BatteryPanel
{
	// Token: 0x02000783 RID: 1923
	public class Opened : BaseBatteryPanelState
	{
		// Token: 0x06002C2D RID: 11309 RVA: 0x000BA768 File Offset: 0x000B8968
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayPodAnimation("Additive", "OpenPanelFinished");
			base.EnablePickup();
		}
	}
}
