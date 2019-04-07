using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055D RID: 1373
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileProximityBeamController : MonoBehaviour
	{
		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06001EA2 RID: 7842 RVA: 0x00090917 File Offset: 0x0008EB17
		private TeamIndex myTeamIndex
		{
			get
			{
				if (!this.teamFilter)
				{
					return TeamIndex.Neutral;
				}
				return this.teamFilter.teamIndex;
			}
		}

		// Token: 0x06001EA3 RID: 7843 RVA: 0x00090934 File Offset: 0x0008EB34
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.projectileController = base.GetComponent<ProjectileController>();
				ProjectileController projectileController = this.projectileController;
				this.teamFilter = ((projectileController != null) ? projectileController.teamFilter : null);
				this.projectileDamage = base.GetComponent<ProjectileDamage>();
				this.attackTimer = 0f;
				this.previousTargets = new List<HealthComponent>();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001EA4 RID: 7844 RVA: 0x00090996 File Offset: 0x0008EB96
		private void ClearList()
		{
			this.previousTargets.Clear();
		}

		// Token: 0x06001EA5 RID: 7845 RVA: 0x000909A3 File Offset: 0x0008EBA3
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateServer();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001EA6 RID: 7846 RVA: 0x000909BC File Offset: 0x0008EBBC
		private void UpdateServer()
		{
			this.listClearTimer -= Time.fixedDeltaTime;
			if (this.listClearTimer <= 0f)
			{
				this.ClearList();
				this.listClearTimer = this.listClearInterval;
			}
			this.attackTimer -= Time.fixedDeltaTime;
			if (this.attackTimer <= 0f)
			{
				this.attackTimer = this.attackInterval;
				Vector3 position = base.transform.position;
				HurtBox hurtBox = this.FindNextTarget(position);
				if (hurtBox)
				{
					this.previousTargets.Add(hurtBox.healthComponent);
					LightningOrb lightningOrb = new LightningOrb();
					lightningOrb.attacker = this.projectileController.owner;
					lightningOrb.teamIndex = this.myTeamIndex;
					lightningOrb.damageValue = this.projectileDamage.damage * this.damageCoefficient;
					lightningOrb.isCrit = this.projectileDamage.crit;
					lightningOrb.origin = position;
					lightningOrb.bouncesRemaining = 0;
					lightningOrb.lightningType = this.lightningType;
					lightningOrb.procCoefficient = this.procCoefficient;
					lightningOrb.target = hurtBox;
					lightningOrb.damageColorIndex = this.projectileDamage.damageColorIndex;
					OrbManager.instance.AddOrb(lightningOrb);
				}
			}
		}

		// Token: 0x06001EA7 RID: 7847 RVA: 0x00090AF0 File Offset: 0x0008ECF0
		public HurtBox FindNextTarget(Vector3 position)
		{
			this.search.searchOrigin = position;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(this.myTeamIndex);
			this.search.filterByLoS = false;
			this.search.maxDistanceFilter = this.attackRange;
			this.search.RefreshCandidates();
			return this.search.GetResults().FirstOrDefault((HurtBox hurtBox) => !this.previousTargets.Contains(hurtBox.healthComponent));
		}

		// Token: 0x04002148 RID: 8520
		private ProjectileController projectileController;

		// Token: 0x04002149 RID: 8521
		private ProjectileDamage projectileDamage;

		// Token: 0x0400214A RID: 8522
		private List<HealthComponent> previousTargets;

		// Token: 0x0400214B RID: 8523
		private TeamFilter teamFilter;

		// Token: 0x0400214C RID: 8524
		public float attackInterval = 1f;

		// Token: 0x0400214D RID: 8525
		public float listClearInterval = 3f;

		// Token: 0x0400214E RID: 8526
		public float attackRange = 20f;

		// Token: 0x0400214F RID: 8527
		public float procCoefficient = 0.1f;

		// Token: 0x04002150 RID: 8528
		public float damageCoefficient = 1f;

		// Token: 0x04002151 RID: 8529
		public LightningOrb.LightningType lightningType = LightningOrb.LightningType.BFG;

		// Token: 0x04002152 RID: 8530
		private float attackTimer;

		// Token: 0x04002153 RID: 8531
		private float listClearTimer;

		// Token: 0x04002154 RID: 8532
		private readonly BullseyeSearch search = new BullseyeSearch();
	}
}
