using System;
using RoR2;

namespace EntityStates.Loader
{
	// Token: 0x020007DF RID: 2015
	public class BeginOvercharge : BaseState
	{
		// Token: 0x06002DE2 RID: 11746 RVA: 0x000C3536 File Offset: 0x000C1736
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				base.GetComponent<LoaderStaticChargeComponent>().ConsumeChargeAuthority();
			}
			this.outer.SetNextStateToMain();
		}
	}
}
