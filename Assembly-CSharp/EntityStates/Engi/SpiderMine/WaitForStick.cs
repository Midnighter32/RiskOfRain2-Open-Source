using System;

namespace EntityStates.Engi.SpiderMine
{
	// Token: 0x0200086B RID: 2155
	public class WaitForStick : BaseSpiderMineState
	{
		// Token: 0x0600309A RID: 12442 RVA: 0x000D17EE File Offset: 0x000CF9EE
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "Idle");
		}

		// Token: 0x0600309B RID: 12443 RVA: 0x000D1806 File Offset: 0x000CFA06
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.projectileStickOnImpact.stuck)
			{
				this.outer.SetNextState(new Burrow());
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x0600309C RID: 12444 RVA: 0x0000B933 File Offset: 0x00009B33
		protected override bool shouldStick
		{
			get
			{
				return true;
			}
		}
	}
}
