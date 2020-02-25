using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000523 RID: 1315
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileProximityBeamController : MonoBehaviour
	{
		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06001F13 RID: 7955 RVA: 0x00086CE7 File Offset: 0x00084EE7
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

		// Token: 0x06001F14 RID: 7956 RVA: 0x00086D04 File Offset: 0x00084F04
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.projectileController = base.GetComponent<ProjectileController>();
				this.teamFilter = this.projectileController.teamFilter;
				this.projectileDamage = base.GetComponent<ProjectileDamage>();
				this.attackTimer = 0f;
				this.previousTargets = new List<HealthComponent>();
				this.search = new BullseyeSearch();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001F15 RID: 7957 RVA: 0x00086D6A File Offset: 0x00084F6A
		private void ClearList()
		{
			this.previousTargets.Clear();
		}

		// Token: 0x06001F16 RID: 7958 RVA: 0x00086D77 File Offset: 0x00084F77
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateServer();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001F17 RID: 7959 RVA: 0x00086D90 File Offset: 0x00084F90
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
				this.attackTimer += this.attackInterval;
				Vector3 position = base.transform.position;
				Vector3 forward = base.transform.forward;
				for (int i = 0; i < this.attackFireCount; i++)
				{
					HurtBox hurtBox = this.FindNextTarget(position, forward);
					if (hurtBox)
					{
						this.previousTargets.Add(hurtBox.healthComponent);
						LightningOrb lightningOrb = new LightningOrb();
						lightningOrb.bouncedObjects = new List<HealthComponent>();
						lightningOrb.attacker = this.projectileController.owner;
						lightningOrb.teamIndex = this.myTeamIndex;
						lightningOrb.damageValue = this.projectileDamage.damage * this.damageCoefficient;
						lightningOrb.isCrit = this.projectileDamage.crit;
						lightningOrb.origin = position;
						lightningOrb.bouncesRemaining = this.bounces;
						lightningOrb.lightningType = this.lightningType;
						lightningOrb.procCoefficient = this.procCoefficient;
						lightningOrb.target = hurtBox;
						lightningOrb.damageColorIndex = this.projectileDamage.damageColorIndex;
						OrbManager.instance.AddOrb(lightningOrb);
					}
				}
			}
		}

		// Token: 0x06001F18 RID: 7960 RVA: 0x00086F0C File Offset: 0x0008510C
		public HurtBox FindNextTarget(Vector3 position, Vector3 forward)
		{
			this.search.searchOrigin = position;
			this.search.searchDirection = forward;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(this.myTeamIndex);
			this.search.filterByLoS = false;
			this.search.minAngleFilter = this.minAngleFilter;
			this.search.maxAngleFilter = this.maxAngleFilter;
			this.search.maxDistanceFilter = this.attackRange;
			this.search.RefreshCandidates();
			return this.search.GetResults().FirstOrDefault((HurtBox hurtBox) => !this.previousTargets.Contains(hurtBox.healthComponent));
		}

		// Token: 0x04001CAD RID: 7341
		private ProjectileController projectileController;

		// Token: 0x04001CAE RID: 7342
		private ProjectileDamage projectileDamage;

		// Token: 0x04001CAF RID: 7343
		private List<HealthComponent> previousTargets;

		// Token: 0x04001CB0 RID: 7344
		private TeamFilter teamFilter;

		// Token: 0x04001CB1 RID: 7345
		public int attackFireCount = 1;

		// Token: 0x04001CB2 RID: 7346
		public float attackInterval = 1f;

		// Token: 0x04001CB3 RID: 7347
		public float listClearInterval = 3f;

		// Token: 0x04001CB4 RID: 7348
		public float attackRange = 20f;

		// Token: 0x04001CB5 RID: 7349
		[Range(0f, 180f)]
		public float minAngleFilter;

		// Token: 0x04001CB6 RID: 7350
		[Range(0f, 180f)]
		public float maxAngleFilter = 180f;

		// Token: 0x04001CB7 RID: 7351
		public float procCoefficient = 0.1f;

		// Token: 0x04001CB8 RID: 7352
		public float damageCoefficient = 1f;

		// Token: 0x04001CB9 RID: 7353
		public int bounces;

		// Token: 0x04001CBA RID: 7354
		public LightningOrb.LightningType lightningType = LightningOrb.LightningType.BFG;

		// Token: 0x04001CBB RID: 7355
		private float attackTimer;

		// Token: 0x04001CBC RID: 7356
		private float listClearTimer;

		// Token: 0x04001CBD RID: 7357
		private BullseyeSearch search;
	}
}
