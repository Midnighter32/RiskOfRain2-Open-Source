using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000518 RID: 1304
	public class LightningStrikeOrb : Orb, IOrbFixedUpdateBehavior
	{
		// Token: 0x06001D50 RID: 7504 RVA: 0x00088CA0 File Offset: 0x00086EA0
		public override void Begin()
		{
			base.duration = 0.5f;
			if (this.target)
			{
				this.lastKnownTargetPosition = this.target.transform.position;
			}
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x00088CD0 File Offset: 0x00086ED0
		public override void OnArrival()
		{
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/LightningStrikeImpact"), new EffectData
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

		// Token: 0x06001D52 RID: 7506 RVA: 0x00088DC4 File Offset: 0x00086FC4
		public void FixedUpdate()
		{
			if (this.target)
			{
				this.lastKnownTargetPosition = this.target.transform.position;
			}
		}

		// Token: 0x04001F96 RID: 8086
		private const float speed = 30f;

		// Token: 0x04001F97 RID: 8087
		public float damageValue;

		// Token: 0x04001F98 RID: 8088
		public GameObject attacker;

		// Token: 0x04001F99 RID: 8089
		public TeamIndex teamIndex;

		// Token: 0x04001F9A RID: 8090
		public bool isCrit;

		// Token: 0x04001F9B RID: 8091
		public float scale;

		// Token: 0x04001F9C RID: 8092
		public ProcChainMask procChainMask;

		// Token: 0x04001F9D RID: 8093
		public float procCoefficient = 0.2f;

		// Token: 0x04001F9E RID: 8094
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001F9F RID: 8095
		private Vector3 lastKnownTargetPosition;
	}
}
