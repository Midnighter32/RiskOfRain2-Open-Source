using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002C6 RID: 710
	[RequireComponent(typeof(TeamFilter))]
	public class DelayBlast : MonoBehaviour
	{
		// Token: 0x06000E66 RID: 3686 RVA: 0x00047168 File Offset: 0x00045368
		private void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x00047178 File Offset: 0x00045378
		private void Start()
		{
			if (this.delayEffect)
			{
				EffectManager.instance.SpawnEffect(this.delayEffect, new EffectData
				{
					origin = base.transform.position,
					rotation = Util.QuaternionSafeLookRotation(base.transform.forward),
					scale = this.radius
				}, true);
			}
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x000471DC File Offset: 0x000453DC
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.timer += Time.fixedDeltaTime;
				if (this.timer >= this.maxTimer)
				{
					EffectManager.instance.SpawnEffect(this.explosionEffect, new EffectData
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
						falloffModel = this.falloffModel
					}.Fire();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04001259 RID: 4697
		[HideInInspector]
		public Vector3 position;

		// Token: 0x0400125A RID: 4698
		[HideInInspector]
		public GameObject attacker;

		// Token: 0x0400125B RID: 4699
		[HideInInspector]
		public GameObject inflictor;

		// Token: 0x0400125C RID: 4700
		[HideInInspector]
		public float baseDamage;

		// Token: 0x0400125D RID: 4701
		[HideInInspector]
		public bool crit;

		// Token: 0x0400125E RID: 4702
		[HideInInspector]
		public float baseForce;

		// Token: 0x0400125F RID: 4703
		[HideInInspector]
		public float radius;

		// Token: 0x04001260 RID: 4704
		[HideInInspector]
		public Vector3 bonusForce;

		// Token: 0x04001261 RID: 4705
		[HideInInspector]
		public float maxTimer;

		// Token: 0x04001262 RID: 4706
		[HideInInspector]
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001263 RID: 4707
		[HideInInspector]
		public BlastAttack.FalloffModel falloffModel;

		// Token: 0x04001264 RID: 4708
		[HideInInspector]
		public DamageType damageType;

		// Token: 0x04001265 RID: 4709
		public GameObject explosionEffect;

		// Token: 0x04001266 RID: 4710
		public GameObject delayEffect;

		// Token: 0x04001267 RID: 4711
		private float timer;

		// Token: 0x04001268 RID: 4712
		private TeamFilter teamFilter;
	}
}
