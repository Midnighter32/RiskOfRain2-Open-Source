using System;
using UnityEngine.Networking;

namespace EntityStates.Engi.Mine
{
	// Token: 0x0200087C RID: 2172
	public class WaitForStick : BaseMineState
	{
		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x060030E4 RID: 12516 RVA: 0x0000B933 File Offset: 0x00009B33
		protected override bool shouldStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x060030E5 RID: 12517 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldRevertToWaitForStickOnSurfaceLost
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060030E6 RID: 12518 RVA: 0x000D2537 File Offset: 0x000D0737
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				base.armingStateMachine.SetNextState(new MineArmingUnarmed());
			}
		}

		// Token: 0x060030E7 RID: 12519 RVA: 0x000D2556 File Offset: 0x000D0756
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.projectileStickOnImpact.stuck)
			{
				this.outer.SetNextState(new Arm());
			}
		}
	}
}
