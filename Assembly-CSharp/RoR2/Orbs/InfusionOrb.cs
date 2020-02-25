using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004CD RID: 1229
	public class InfusionOrb : Orb
	{
		// Token: 0x06001D76 RID: 7542 RVA: 0x0007DA2C File Offset: 0x0007BC2C
		public override void Begin()
		{
			base.duration = base.distanceToTarget / 30f;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), effectData, true);
			HurtBox component = this.target.GetComponent<HurtBox>();
			CharacterBody characterBody = (component != null) ? component.healthComponent.GetComponent<CharacterBody>() : null;
			if (characterBody)
			{
				this.targetInventory = characterBody.inventory;
			}
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x0007DAB7 File Offset: 0x0007BCB7
		public override void OnArrival()
		{
			if (this.targetInventory)
			{
				this.targetInventory.AddInfusionBonus((uint)this.maxHpValue);
			}
		}

		// Token: 0x04001A8E RID: 6798
		private const float speed = 30f;

		// Token: 0x04001A8F RID: 6799
		public int maxHpValue;

		// Token: 0x04001A90 RID: 6800
		private Inventory targetInventory;
	}
}
