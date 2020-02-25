using System;
using UnityEngine.Networking;

namespace EntityStates.Engi.Mine
{
	// Token: 0x02000879 RID: 2169
	public class MineArmingWeak : BaseMineArmingState
	{
		// Token: 0x060030D8 RID: 12504 RVA: 0x000D2447 File Offset: 0x000D0647
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && MineArmingWeak.duration <= base.fixedAge)
			{
				this.outer.SetNextState(new MineArmingFull());
			}
		}

		// Token: 0x04002F1A RID: 12058
		public static float duration;
	}
}
