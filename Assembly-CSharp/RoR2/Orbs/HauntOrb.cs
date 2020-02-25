using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004CB RID: 1227
	public class HauntOrb : Orb
	{
		// Token: 0x06001D70 RID: 7536 RVA: 0x0007D870 File Offset: 0x0007BA70
		public override void Begin()
		{
			base.duration = this.timeToArrive + UnityEngine.Random.Range(0f, 0.4f);
			EffectData effectData = new EffectData
			{
				scale = this.scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/HauntOrbEffect"), effectData, true);
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x0007D8E0 File Offset: 0x0007BAE0
		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent && healthComponent.body)
				{
					healthComponent.body.AddTimedBuff(BuffIndex.AffixHaunted, 15f);
				}
			}
		}

		// Token: 0x04001A89 RID: 6793
		public float timeToArrive;

		// Token: 0x04001A8A RID: 6794
		public float scale;
	}
}
