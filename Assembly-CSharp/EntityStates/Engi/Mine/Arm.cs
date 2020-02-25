using System;
using UnityEngine.Networking;

namespace EntityStates.Engi.Mine
{
	// Token: 0x0200087D RID: 2173
	public class Arm : BaseMineState
	{
		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x060030E9 RID: 12521 RVA: 0x0000B933 File Offset: 0x00009B33
		protected override bool shouldStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060030EA RID: 12522 RVA: 0x000D258A File Offset: 0x000D078A
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && Arm.duration <= base.fixedAge)
			{
				this.outer.SetNextState(new WaitForTarget());
			}
		}

		// Token: 0x04002F1E RID: 12062
		public static float duration;
	}
}
