using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TeleporterHealNovaController
{
	// Token: 0x02000776 RID: 1910
	public class TeleporterHealNovaPulse : BaseState
	{
		// Token: 0x06002BF6 RID: 11254 RVA: 0x000B9BAC File Offset: 0x000B7DAC
		public override void OnEnter()
		{
			base.OnEnter();
			Transform parent = base.transform.parent;
			if (parent)
			{
				TeleporterInteraction component = parent.GetComponent<TeleporterInteraction>();
				if (component)
				{
					this.radius = component.clearRadius;
				}
			}
			TeamFilter component2 = base.GetComponent<TeamFilter>();
			TeamIndex teamIndex = component2 ? component2.teamIndex : TeamIndex.None;
			if (NetworkServer.active)
			{
				this.healPulse = new TeleporterHealNovaPulse.HealPulse(base.transform.position, this.radius, 0.5f, TeleporterHealNovaPulse.duration, teamIndex);
			}
			this.effectTransform = base.transform.Find("PulseEffect");
			if (this.effectTransform)
			{
				this.effectTransform.gameObject.SetActive(true);
			}
		}

		// Token: 0x06002BF7 RID: 11255 RVA: 0x000B9C69 File Offset: 0x000B7E69
		public override void OnExit()
		{
			if (this.effectTransform)
			{
				this.effectTransform.gameObject.SetActive(false);
			}
			base.OnExit();
		}

		// Token: 0x06002BF8 RID: 11256 RVA: 0x000B9C8F File Offset: 0x000B7E8F
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				this.healPulse.Update(Time.fixedDeltaTime);
				if (TeleporterHealNovaPulse.duration < base.fixedAge)
				{
					EntityState.Destroy(this.outer.gameObject);
				}
			}
		}

		// Token: 0x06002BF9 RID: 11257 RVA: 0x000B9CCC File Offset: 0x000B7ECC
		public override void Update()
		{
			if (this.effectTransform)
			{
				float num = this.radius * TeleporterHealNovaPulse.novaRadiusCurve.Evaluate(base.fixedAge / TeleporterHealNovaPulse.duration);
				this.effectTransform.localScale = new Vector3(num, num, num);
			}
		}

		// Token: 0x04002811 RID: 10257
		public static AnimationCurve novaRadiusCurve;

		// Token: 0x04002812 RID: 10258
		public static float duration;

		// Token: 0x04002813 RID: 10259
		private Transform effectTransform;

		// Token: 0x04002814 RID: 10260
		private TeleporterHealNovaPulse.HealPulse healPulse;

		// Token: 0x04002815 RID: 10261
		private float radius;

		// Token: 0x02000777 RID: 1911
		private class HealPulse
		{
			// Token: 0x06002BFB RID: 11259 RVA: 0x000B9D18 File Offset: 0x000B7F18
			public HealPulse(Vector3 origin, float finalRadius, float healFractionValue, float duration, TeamIndex teamIndex)
			{
				this.sphereSearch = new SphereSearch
				{
					mask = LayerIndex.entityPrecise.mask,
					origin = origin,
					queryTriggerInteraction = QueryTriggerInteraction.Collide,
					radius = 0f
				};
				this.finalRadius = finalRadius;
				this.healFractionValue = healFractionValue;
				this.rate = 1f / duration;
				this.teamMask = default(TeamMask);
				this.teamMask.AddTeam(teamIndex);
			}

			// Token: 0x17000423 RID: 1059
			// (get) Token: 0x06002BFC RID: 11260 RVA: 0x000B9DAD File Offset: 0x000B7FAD
			public bool isFinished
			{
				get
				{
					return this.t >= 1f;
				}
			}

			// Token: 0x06002BFD RID: 11261 RVA: 0x000B9DC0 File Offset: 0x000B7FC0
			public void Update(float deltaTime)
			{
				this.t += this.rate * deltaTime;
				this.t = ((this.t > 1f) ? 1f : this.t);
				this.sphereSearch.radius = this.finalRadius * TeleporterHealNovaPulse.novaRadiusCurve.Evaluate(this.t);
				this.sphereSearch.RefreshCandidates().FilterCandidatesByHurtBoxTeam(this.teamMask).FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes(this.hurtBoxesList);
				int i = 0;
				int count = this.hurtBoxesList.Count;
				while (i < count)
				{
					HealthComponent healthComponent = this.hurtBoxesList[i].healthComponent;
					if (!this.healedTargets.Contains(healthComponent))
					{
						this.healedTargets.Add(healthComponent);
						this.HealTarget(healthComponent);
					}
					i++;
				}
				this.hurtBoxesList.Clear();
			}

			// Token: 0x06002BFE RID: 11262 RVA: 0x000B9EA0 File Offset: 0x000B80A0
			private void HealTarget(HealthComponent target)
			{
				target.HealFraction(this.healFractionValue, default(ProcChainMask));
				Util.PlaySound("Play_item_proc_TPhealingNova_hitPlayer", target.gameObject);
			}

			// Token: 0x04002816 RID: 10262
			private readonly List<HealthComponent> healedTargets = new List<HealthComponent>();

			// Token: 0x04002817 RID: 10263
			private readonly SphereSearch sphereSearch;

			// Token: 0x04002818 RID: 10264
			private float rate;

			// Token: 0x04002819 RID: 10265
			private float t;

			// Token: 0x0400281A RID: 10266
			private float finalRadius;

			// Token: 0x0400281B RID: 10267
			private float healFractionValue;

			// Token: 0x0400281C RID: 10268
			private TeamMask teamMask;

			// Token: 0x0400281D RID: 10269
			private readonly List<HurtBox> hurtBoxesList = new List<HurtBox>();
		}
	}
}
