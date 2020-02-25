using System;
using EntityStates.Engi.EngiBubbleShield;
using UnityEngine;

namespace EntityStates.Engi.EngiWallShield
{
	// Token: 0x02000890 RID: 2192
	public class Undeployed : Undeployed
	{
		// Token: 0x0600313B RID: 12603 RVA: 0x000D40BB File Offset: 0x000D22BB
		public override void OnEnter()
		{
			base.OnEnter();
		}

		// Token: 0x0600313C RID: 12604 RVA: 0x000D40C4 File Offset: 0x000D22C4
		protected override void SetNextState()
		{
			Vector3 forward = base.transform.forward;
			Vector3 forward2 = new Vector3(forward.x, 0f, forward.z);
			base.transform.forward = forward2;
			this.outer.SetNextState(new Deployed());
		}
	}
}
