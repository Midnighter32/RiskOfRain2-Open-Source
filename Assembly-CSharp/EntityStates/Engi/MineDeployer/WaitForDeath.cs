using System;
using UnityEngine.Networking;

namespace EntityStates.Engi.MineDeployer
{
	// Token: 0x02000875 RID: 2165
	public class WaitForDeath : BaseMineDeployerState
	{
		// Token: 0x060030CF RID: 12495 RVA: 0x000D2344 File Offset: 0x000D0544
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && WaitForDeath.duration <= base.fixedAge)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x04002F10 RID: 12048
		public static float duration;
	}
}
