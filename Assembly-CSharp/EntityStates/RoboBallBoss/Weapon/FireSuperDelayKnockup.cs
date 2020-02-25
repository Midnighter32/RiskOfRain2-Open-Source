using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.RoboBallBoss.Weapon
{
	// Token: 0x020007A2 RID: 1954
	public class FireSuperDelayKnockup : FireDelayKnockup
	{
		// Token: 0x06002CB7 RID: 11447 RVA: 0x000BCBCB File Offset: 0x000BADCB
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				base.characterBody.AddTimedBuff(BuffIndex.EngiShield, FireSuperDelayKnockup.shieldDuration);
			}
		}

		// Token: 0x040028E0 RID: 10464
		public static float shieldDuration;
	}
}
