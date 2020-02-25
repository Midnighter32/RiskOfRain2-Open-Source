using System;

namespace EntityStates.Interactables.GoldBeacon
{
	// Token: 0x02000812 RID: 2066
	public class NotReady : GoldBeaconBaseState
	{
		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06002EE3 RID: 12003 RVA: 0x000C7A5B File Offset: 0x000C5C5B
		// (set) Token: 0x06002EE4 RID: 12004 RVA: 0x000C7A62 File Offset: 0x000C5C62
		public static int count { get; private set; }

		// Token: 0x06002EE5 RID: 12005 RVA: 0x000C7A6A File Offset: 0x000C5C6A
		public override void OnEnter()
		{
			base.OnEnter();
			base.SetReady(false);
			NotReady.count++;
		}

		// Token: 0x06002EE6 RID: 12006 RVA: 0x000C7A85 File Offset: 0x000C5C85
		public override void OnExit()
		{
			NotReady.count--;
			base.OnExit();
		}
	}
}
