using System;

namespace EntityStates.Barrel
{
	// Token: 0x020008F8 RID: 2296
	public class Opened : EntityState
	{
		// Token: 0x0600334E RID: 13134 RVA: 0x000DE8F3 File Offset: 0x000DCAF3
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Opened");
		}
	}
}
