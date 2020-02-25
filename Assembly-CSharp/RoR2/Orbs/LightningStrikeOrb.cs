using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004D2 RID: 1234
	public class LightningStrikeOrb : Orb, IOrbFixedUpdateBehavior
	{
		// Token: 0x06001D81 RID: 7553 RVA: 0x0007DFE3 File Offset: 0x0007C1E3
		public override void Begin()
		{
			base.duration = 0.5f;
			if (this.target)
			{
				this.lastKnownTargetPosition = this.target.transform.position;
			}
		}

		// Token: 0x06001D82 RID: 7554 RVA: 0x0007E014 File Offset: 0x0007C214
		public override void OnArrival()
		{
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/LightningStrikeImpact"), new EffectData
			{
				origin = this.lastKnownTargetPosition
			}, true);
			if (this.attacker)
			{
				new BlastAttack
				{
					attacker = this.attacker,
					baseDamage = this.damageValue,
					baseForce = 0f,
					bonusForce = Vector3.down * 3000f,
					canHurtAttacker = false,
					crit = this.isCrit,
					damageColorIndex = DamageColorIndex.Item,
					damageType = DamageType.Stun1s,
					falloffModel = BlastAttack.FalloffModel.None,
					inflictor = null,
					position = this.lastKnownTargetPosition,
					procChainMask = default(ProcChainMask),
					procCoefficient = 1f,
					radius = 3f,
					teamIndex = TeamComponent.GetObjectTeam(this.attacker)
				}.Fire();
			}
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x0007E104 File Offset: 0x0007C304
		public void FixedUpdate()
		{
			if (this.target)
			{
				this.lastKnownTargetPosition = this.target.transform.position;
			}
		}

		// Token: 0x04001AAE RID: 6830
		private const float speed = 30f;

		// Token: 0x04001AAF RID: 6831
		public float damageValue;

		// Token: 0x04001AB0 RID: 6832
		public GameObject attacker;

		// Token: 0x04001AB1 RID: 6833
		public TeamIndex teamIndex;

		// Token: 0x04001AB2 RID: 6834
		public bool isCrit;

		// Token: 0x04001AB3 RID: 6835
		public float scale;

		// Token: 0x04001AB4 RID: 6836
		public ProcChainMask procChainMask;

		// Token: 0x04001AB5 RID: 6837
		public float procCoefficient = 0.2f;

		// Token: 0x04001AB6 RID: 6838
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001AB7 RID: 6839
		private Vector3 lastKnownTargetPosition;
	}
}
