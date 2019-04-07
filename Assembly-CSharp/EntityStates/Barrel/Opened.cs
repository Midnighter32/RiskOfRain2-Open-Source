using System;

namespace EntityStates.Barrel
{
	// Token: 0x020001DD RID: 477
	public class Opened : EntityState
	{
		// Token: 0x0600094E RID: 2382 RVA: 0x0002ECCF File Offset: 0x0002CECF
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Opened");
		}
	}
}
