using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004CC RID: 1228
	public class HealOrb : Orb
	{
		// Token: 0x06001D73 RID: 7539 RVA: 0x0007D930 File Offset: 0x0007BB30
		public override void Begin()
		{
			if (this.target)
			{
				base.duration = this.overrideDuration;
				float scale = this.scaleOrb ? Mathf.Min(this.healValue / this.target.healthComponent.fullHealth, 1f) : 1f;
				EffectData effectData = new EffectData
				{
					scale = scale,
					origin = this.origin,
					genericFloat = base.duration
				};
				effectData.SetHurtBoxReference(this.target);
				EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/HealthOrbEffect"), effectData, true);
			}
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x0007D9CC File Offset: 0x0007BBCC
		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					healthComponent.Heal(this.healValue, default(ProcChainMask), true);
				}
			}
		}

		// Token: 0x04001A8B RID: 6795
		public float healValue;

		// Token: 0x04001A8C RID: 6796
		public bool scaleOrb = true;

		// Token: 0x04001A8D RID: 6797
		public float overrideDuration = 0.3f;
	}
}
