using System;
using UnityEngine;

namespace EntityStates.ClayBoss
{
	// Token: 0x020008D7 RID: 2263
	public class RecoverExit : BaseState
	{
		// Token: 0x060032AF RID: 12975 RVA: 0x000DB705 File Offset: 0x000D9905
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			base.PlayAnimation("Body", "ExitSiphon", "ExitSiphon.playbackRate", RecoverExit.exitDuration);
		}

		// Token: 0x060032B0 RID: 12976 RVA: 0x000DB732 File Offset: 0x000D9932
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= RecoverExit.exitDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x040031D2 RID: 12754
		public static float exitDuration = 1f;

		// Token: 0x040031D3 RID: 12755
		private float stopwatch;
	}
}
