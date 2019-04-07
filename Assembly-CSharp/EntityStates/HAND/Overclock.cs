using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.HAND
{
	// Token: 0x02000162 RID: 354
	public class Overclock : BaseState
	{
		// Token: 0x060006E0 RID: 1760 RVA: 0x00020E5B File Offset: 0x0001F05B
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				base.characterBody;
			}
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x00020E76 File Offset: 0x0001F076
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > Overclock.baseDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x04000871 RID: 2161
		public static float baseDuration = 0.25f;

		// Token: 0x04000872 RID: 2162
		public static GameObject healEffectPrefab;

		// Token: 0x04000873 RID: 2163
		public static float healPercentage = 0.15f;
	}
}
