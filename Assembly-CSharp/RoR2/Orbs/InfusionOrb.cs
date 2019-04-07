using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000513 RID: 1299
	public class InfusionOrb : Orb
	{
		// Token: 0x06001D45 RID: 7493 RVA: 0x0008874C File Offset: 0x0008694C
		public override void Begin()
		{
			base.duration = base.distanceToTarget / 30f;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), effectData, true);
			HurtBox component = this.target.GetComponent<HurtBox>();
			CharacterBody characterBody = (component != null) ? component.healthComponent.GetComponent<CharacterBody>() : null;
			if (characterBody)
			{
				this.targetInventory = characterBody.inventory;
			}
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x000887DC File Offset: 0x000869DC
		public override void OnArrival()
		{
			if (this.targetInventory)
			{
				this.targetInventory.AddInfusionBonus((uint)this.maxHpValue);
				HurtBox component = this.target.GetComponent<HurtBox>();
				HealthComponent healthComponent = (component != null) ? component.healthComponent : null;
				if (healthComponent)
				{
					healthComponent.Heal((float)this.maxHpValue, default(ProcChainMask), true);
				}
			}
		}

		// Token: 0x04001F7B RID: 8059
		private const float speed = 30f;

		// Token: 0x04001F7C RID: 8060
		public int maxHpValue;

		// Token: 0x04001F7D RID: 8061
		private Inventory targetInventory;
	}
}
