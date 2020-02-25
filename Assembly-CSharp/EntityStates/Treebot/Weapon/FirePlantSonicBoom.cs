using System;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x02000754 RID: 1876
	public class FirePlantSonicBoom : FireSonicBoom
	{
		// Token: 0x06002B70 RID: 11120 RVA: 0x000B733C File Offset: 0x000B553C
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active && base.healthComponent && FirePlantSonicBoom.healthCostFraction >= Mathf.Epsilon)
			{
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = base.healthComponent.combinedHealth * FirePlantSonicBoom.healthCostFraction;
				damageInfo.position = base.characterBody.corePosition;
				damageInfo.force = Vector3.zero;
				damageInfo.damageColorIndex = DamageColorIndex.Default;
				damageInfo.crit = false;
				damageInfo.attacker = null;
				damageInfo.inflictor = null;
				damageInfo.damageType = DamageType.NonLethal;
				damageInfo.procCoefficient = 0f;
				damageInfo.procChainMask = default(ProcChainMask);
				base.healthComponent.TakeDamage(damageInfo);
			}
		}

		// Token: 0x06002B71 RID: 11121 RVA: 0x000B73F4 File Offset: 0x000B55F4
		protected override void AddDebuff(CharacterBody body)
		{
			SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
			if (component != null)
			{
				component.SetStun(-1f);
			}
			if (FirePlantSonicBoom.hitEffectPrefab)
			{
				EffectManager.SpawnEffect(FirePlantSonicBoom.hitEffectPrefab, new EffectData
				{
					origin = body.corePosition
				}, true);
			}
			if (base.healthComponent)
			{
				HealOrb healOrb = new HealOrb();
				healOrb.origin = body.corePosition;
				healOrb.target = base.healthComponent.body.mainHurtBox;
				healOrb.healValue = FirePlantSonicBoom.healthFractionPerHit * base.healthComponent.fullHealth;
				healOrb.overrideDuration = UnityEngine.Random.Range(0.3f, 0.6f);
				OrbManager.instance.AddOrb(healOrb);
			}
			Util.PlaySound(FirePlantSonicBoom.impactSoundString, base.gameObject);
		}

		// Token: 0x06002B72 RID: 11122 RVA: 0x000B74C1 File Offset: 0x000B56C1
		protected override float CalculateDamage()
		{
			return FirePlantSonicBoom.damageCoefficient * this.damageStat;
		}

		// Token: 0x06002B73 RID: 11123 RVA: 0x000B74CF File Offset: 0x000B56CF
		protected override float CalculateProcCoefficient()
		{
			return FirePlantSonicBoom.procCoefficient;
		}

		// Token: 0x04002771 RID: 10097
		public static float damageCoefficient;

		// Token: 0x04002772 RID: 10098
		public static float procCoefficient;

		// Token: 0x04002773 RID: 10099
		public static GameObject hitEffectPrefab;

		// Token: 0x04002774 RID: 10100
		public static float healthFractionPerHit;

		// Token: 0x04002775 RID: 10101
		public static float healthCostFraction;

		// Token: 0x04002776 RID: 10102
		public static string impactSoundString;
	}
}
