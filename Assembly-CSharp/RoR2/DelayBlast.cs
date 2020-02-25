using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001D5 RID: 469
	[RequireComponent(typeof(TeamFilter))]
	public class DelayBlast : MonoBehaviour
	{
		// Token: 0x06000A09 RID: 2569 RVA: 0x0002BF2F File Offset: 0x0002A12F
		private void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x0002BF40 File Offset: 0x0002A140
		private void Start()
		{
			if (this.delayEffect)
			{
				EffectManager.SpawnEffect(this.delayEffect, new EffectData
				{
					origin = base.transform.position,
					rotation = Util.QuaternionSafeLookRotation(base.transform.forward),
					scale = this.radius
				}, true);
			}
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x0002BFA0 File Offset: 0x0002A1A0
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.timer += Time.fixedDeltaTime;
				if (this.timer >= this.maxTimer)
				{
					EffectManager.SpawnEffect(this.explosionEffect, new EffectData
					{
						origin = base.transform.position,
						rotation = Util.QuaternionSafeLookRotation(base.transform.forward),
						scale = this.radius
					}, true);
					new BlastAttack
					{
						position = this.position,
						baseDamage = this.baseDamage,
						baseForce = this.baseForce,
						bonusForce = this.bonusForce,
						radius = this.radius,
						attacker = this.attacker,
						inflictor = this.inflictor,
						teamIndex = this.teamFilter.teamIndex,
						crit = this.crit,
						damageColorIndex = this.damageColorIndex,
						damageType = this.damageType,
						falloffModel = this.falloffModel,
						procCoefficient = this.procCoefficient
					}.Fire();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04000A43 RID: 2627
		[HideInInspector]
		public Vector3 position;

		// Token: 0x04000A44 RID: 2628
		[HideInInspector]
		public GameObject attacker;

		// Token: 0x04000A45 RID: 2629
		[HideInInspector]
		public GameObject inflictor;

		// Token: 0x04000A46 RID: 2630
		[HideInInspector]
		public float baseDamage;

		// Token: 0x04000A47 RID: 2631
		[HideInInspector]
		public bool crit;

		// Token: 0x04000A48 RID: 2632
		[HideInInspector]
		public float baseForce;

		// Token: 0x04000A49 RID: 2633
		[HideInInspector]
		public float radius;

		// Token: 0x04000A4A RID: 2634
		[HideInInspector]
		public Vector3 bonusForce;

		// Token: 0x04000A4B RID: 2635
		[HideInInspector]
		public float maxTimer;

		// Token: 0x04000A4C RID: 2636
		[HideInInspector]
		public DamageColorIndex damageColorIndex;

		// Token: 0x04000A4D RID: 2637
		[HideInInspector]
		public BlastAttack.FalloffModel falloffModel;

		// Token: 0x04000A4E RID: 2638
		[HideInInspector]
		public DamageType damageType;

		// Token: 0x04000A4F RID: 2639
		[HideInInspector]
		public float procCoefficient = 1f;

		// Token: 0x04000A50 RID: 2640
		public GameObject explosionEffect;

		// Token: 0x04000A51 RID: 2641
		public GameObject delayEffect;

		// Token: 0x04000A52 RID: 2642
		private float timer;

		// Token: 0x04000A53 RID: 2643
		private TeamFilter teamFilter;
	}
}
