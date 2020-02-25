using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004D0 RID: 1232
	public class LightningOrb : Orb
	{
		// Token: 0x06001D7C RID: 7548 RVA: 0x0007DB44 File Offset: 0x0007BD44
		public override void Begin()
		{
			base.duration = 0.1f;
			string path = null;
			switch (this.lightningType)
			{
			case LightningOrb.LightningType.Ukulele:
				path = "Prefabs/Effects/OrbEffects/LightningOrbEffect";
				break;
			case LightningOrb.LightningType.Tesla:
				path = "Prefabs/Effects/OrbEffects/TeslaOrbEffect";
				break;
			case LightningOrb.LightningType.BFG:
				path = "Prefabs/Effects/OrbEffects/BeamSphereOrbEffect";
				base.duration = 0.4f;
				break;
			case LightningOrb.LightningType.TreePoisonDart:
				path = "Prefabs/Effects/OrbEffects/TreePoisonDartOrbEffect";
				this.speed = 40f;
				base.duration = base.distanceToTarget / this.speed;
				break;
			case LightningOrb.LightningType.HuntressGlaive:
				path = "Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect";
				base.duration = base.distanceToTarget / this.speed;
				this.canBounceOnSameTarget = true;
				break;
			case LightningOrb.LightningType.Loader:
				path = "Prefabs/Effects/OrbEffects/LoaderLightningOrbEffect";
				break;
			case LightningOrb.LightningType.RazorWire:
				path = "Prefabs/Effects/OrbEffects/RazorwireOrbEffect";
				base.duration = 0.2f;
				break;
			case LightningOrb.LightningType.CrocoDisease:
				path = "Prefabs/Effects/OrbEffects/CrocoDiseaseOrbEffect";
				base.duration = 0.6f;
				this.targetsToFindPerBounce = 2;
				this.damageType = DamageType.PoisonOnHit;
				break;
			}
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.SpawnEffect(Resources.Load<GameObject>(path), effectData, true);
		}

		// Token: 0x06001D7D RID: 7549 RVA: 0x0007DC7C File Offset: 0x0007BE7C
		public override void OnArrival()
		{
			if (this.target)
			{
				for (int i = 0; i < this.targetsToFindPerBounce; i++)
				{
					if (this.bouncesRemaining > 0)
					{
						if (this.canBounceOnSameTarget)
						{
							this.bouncedObjects.Clear();
						}
						if (this.bouncedObjects != null)
						{
							this.bouncedObjects.Add(this.target.healthComponent);
						}
						HurtBox hurtBox = this.PickNextTarget(this.target.transform.position);
						if (hurtBox)
						{
							LightningOrb lightningOrb = new LightningOrb();
							lightningOrb.search = this.search;
							lightningOrb.origin = this.target.transform.position;
							lightningOrb.target = hurtBox;
							lightningOrb.attacker = this.attacker;
							lightningOrb.teamIndex = this.teamIndex;
							lightningOrb.damageValue = this.damageValue * this.damageCoefficientPerBounce;
							lightningOrb.bouncesRemaining = this.bouncesRemaining - 1;
							lightningOrb.isCrit = this.isCrit;
							lightningOrb.bouncedObjects = this.bouncedObjects;
							lightningOrb.lightningType = this.lightningType;
							lightningOrb.procChainMask = this.procChainMask;
							lightningOrb.procCoefficient = this.procCoefficient;
							lightningOrb.damageColorIndex = this.damageColorIndex;
							lightningOrb.damageCoefficientPerBounce = this.damageCoefficientPerBounce;
							lightningOrb.speed = this.speed;
							lightningOrb.range = this.range;
							OrbManager.instance.AddOrb(lightningOrb);
						}
					}
				}
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = this.damageValue;
					damageInfo.attacker = this.attacker;
					damageInfo.inflictor = null;
					damageInfo.force = Vector3.zero;
					damageInfo.crit = this.isCrit;
					damageInfo.procChainMask = this.procChainMask;
					damageInfo.procCoefficient = this.procCoefficient;
					damageInfo.position = this.target.transform.position;
					damageInfo.damageColorIndex = this.damageColorIndex;
					damageInfo.damageType = this.damageType;
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
				}
			}
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x0007DEC0 File Offset: 0x0007C0C0
		public HurtBox PickNextTarget(Vector3 position)
		{
			if (this.search == null)
			{
				this.search = new BullseyeSearch();
			}
			this.search.searchOrigin = position;
			this.search.searchDirection = Vector3.zero;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(this.teamIndex);
			this.search.filterByLoS = false;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.range;
			this.search.RefreshCandidates();
			HurtBox hurtBox = (from v in this.search.GetResults()
			where !this.bouncedObjects.Contains(v.healthComponent)
			select v).FirstOrDefault<HurtBox>();
			if (hurtBox)
			{
				this.bouncedObjects.Add(hurtBox.healthComponent);
			}
			return hurtBox;
		}

		// Token: 0x04001A93 RID: 6803
		public float speed = 100f;

		// Token: 0x04001A94 RID: 6804
		public float damageValue;

		// Token: 0x04001A95 RID: 6805
		public GameObject attacker;

		// Token: 0x04001A96 RID: 6806
		public int bouncesRemaining;

		// Token: 0x04001A97 RID: 6807
		public List<HealthComponent> bouncedObjects;

		// Token: 0x04001A98 RID: 6808
		public TeamIndex teamIndex;

		// Token: 0x04001A99 RID: 6809
		public bool isCrit;

		// Token: 0x04001A9A RID: 6810
		public ProcChainMask procChainMask;

		// Token: 0x04001A9B RID: 6811
		public float procCoefficient = 1f;

		// Token: 0x04001A9C RID: 6812
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001A9D RID: 6813
		public float range = 20f;

		// Token: 0x04001A9E RID: 6814
		public float damageCoefficientPerBounce = 1f;

		// Token: 0x04001A9F RID: 6815
		public int targetsToFindPerBounce = 1;

		// Token: 0x04001AA0 RID: 6816
		public DamageType damageType;

		// Token: 0x04001AA1 RID: 6817
		private bool canBounceOnSameTarget;

		// Token: 0x04001AA2 RID: 6818
		public LightningOrb.LightningType lightningType;

		// Token: 0x04001AA3 RID: 6819
		private BullseyeSearch search;

		// Token: 0x020004D1 RID: 1233
		public enum LightningType
		{
			// Token: 0x04001AA5 RID: 6821
			Ukulele,
			// Token: 0x04001AA6 RID: 6822
			Tesla,
			// Token: 0x04001AA7 RID: 6823
			BFG,
			// Token: 0x04001AA8 RID: 6824
			TreePoisonDart,
			// Token: 0x04001AA9 RID: 6825
			HuntressGlaive,
			// Token: 0x04001AAA RID: 6826
			Loader,
			// Token: 0x04001AAB RID: 6827
			RazorWire,
			// Token: 0x04001AAC RID: 6828
			CrocoDisease,
			// Token: 0x04001AAD RID: 6829
			Count
		}
	}
}
