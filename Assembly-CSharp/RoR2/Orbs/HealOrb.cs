using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000512 RID: 1298
	public class HealOrb : Orb
	{
		// Token: 0x06001D42 RID: 7490 RVA: 0x00088674 File Offset: 0x00086874
		public override void Begin()
		{
			float scale = this.scaleOrb ? (this.healValue / 10f) : 1f;
			base.duration = base.distanceToTarget / 20f;
			EffectData effectData = new EffectData
			{
				scale = scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/HealthOrbEffect"), effectData, true);
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x000886F8 File Offset: 0x000868F8
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

		// Token: 0x04001F78 RID: 8056
		private const float speed = 20f;

		// Token: 0x04001F79 RID: 8057
		public float healValue;

		// Token: 0x04001F7A RID: 8058
		public bool scaleOrb = true;
	}
}
