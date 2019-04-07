using System;
using UnityEngine;

namespace EntityStates.ClayBoss
{
	// Token: 0x020001BC RID: 444
	public class RecoverExit : BaseState
	{
		// Token: 0x060008B0 RID: 2224 RVA: 0x0002BAD1 File Offset: 0x00029CD1
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			base.PlayAnimation("Body", "ExitSiphon", "ExitSiphon.playbackRate", RecoverExit.exitDuration);
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x0002BAFE File Offset: 0x00029CFE
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

		// Token: 0x04000BAA RID: 2986
		public static float exitDuration = 1f;

		// Token: 0x04000BAB RID: 2987
		private float stopwatch;
	}
}
