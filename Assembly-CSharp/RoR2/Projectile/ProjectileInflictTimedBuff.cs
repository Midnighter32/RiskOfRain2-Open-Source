using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000517 RID: 1303
	public class ProjectileInflictTimedBuff : MonoBehaviour, IOnDamageInflictedServerReceiver
	{
		// Token: 0x06001EC5 RID: 7877 RVA: 0x00085404 File Offset: 0x00083604
		public void OnDamageInflictedServer(DamageReport damageReport)
		{
			CharacterBody victimBody = damageReport.victimBody;
			if (victimBody)
			{
				victimBody.AddTimedBuff(this.buffIndex, this.duration);
			}
		}

		// Token: 0x04001C4F RID: 7247
		public BuffIndex buffIndex;

		// Token: 0x04001C50 RID: 7248
		public float duration;
	}
}
