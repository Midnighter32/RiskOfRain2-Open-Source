using System;

namespace EntityStates.Interactables.GoldBeacon
{
	// Token: 0x0200013B RID: 315
	public class NotReady : GoldBeaconBaseState
	{
		// Token: 0x06000608 RID: 1544 RVA: 0x0001BCFB File Offset: 0x00019EFB
		public override void OnEnter()
		{
			base.OnEnter();
			base.SetReady(false);
		}
	}
}
