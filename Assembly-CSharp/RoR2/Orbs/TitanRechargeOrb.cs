using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004D8 RID: 1240
	public class TitanRechargeOrb : Orb
	{
		// Token: 0x06001DA6 RID: 7590 RVA: 0x0007E970 File Offset: 0x0007CB70
		public override void Begin()
		{
			base.duration = 1f;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/HealthOrbEffect"), effectData, true);
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnArrival()
		{
		}

		// Token: 0x04001ADE RID: 6878
		public int targetRockInt;

		// Token: 0x04001ADF RID: 6879
		public TitanRockController titanRockController;
	}
}
