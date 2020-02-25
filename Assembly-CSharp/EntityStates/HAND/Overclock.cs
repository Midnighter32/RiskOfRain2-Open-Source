using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.HAND
{
	// Token: 0x02000844 RID: 2116
	public class Overclock : BaseState
	{
		// Token: 0x06002FE5 RID: 12261 RVA: 0x000CD56D File Offset: 0x000CB76D
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				base.characterBody;
			}
		}

		// Token: 0x06002FE6 RID: 12262 RVA: 0x000CD588 File Offset: 0x000CB788
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > Overclock.baseDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x04002DB7 RID: 11703
		public static float baseDuration = 0.25f;

		// Token: 0x04002DB8 RID: 11704
		public static GameObject healEffectPrefab;

		// Token: 0x04002DB9 RID: 11705
		public static float healPercentage = 0.15f;
	}
}
