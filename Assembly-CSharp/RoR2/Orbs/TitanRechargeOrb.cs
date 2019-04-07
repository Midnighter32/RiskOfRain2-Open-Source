using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200051E RID: 1310
	public class TitanRechargeOrb : Orb
	{
		// Token: 0x06001D75 RID: 7541 RVA: 0x00089634 File Offset: 0x00087834
		public override void Begin()
		{
			base.duration = 1f;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/HealthOrbEffect"), effectData, true);
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnArrival()
		{
		}

		// Token: 0x04001FC5 RID: 8133
		public int targetRockInt;

		// Token: 0x04001FC6 RID: 8134
		public TitanRockController titanRockController;
	}
}
