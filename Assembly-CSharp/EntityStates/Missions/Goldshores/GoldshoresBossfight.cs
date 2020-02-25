using System;
using EntityStates.Interactables.GoldBeacon;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Missions.Goldshores
{
	// Token: 0x020007BA RID: 1978
	public class GoldshoresBossfight : EntityState
	{
		// Token: 0x06002D2F RID: 11567 RVA: 0x000BEC86 File Offset: 0x000BCE86
		public override void OnEnter()
		{
			base.OnEnter();
			this.missionController = base.GetComponent<GoldshoresMissionController>();
			this.bossInvulnerabilityStartTime = Run.FixedTimeStamp.negativeInfinity;
		}

		// Token: 0x06002D30 RID: 11568 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002D31 RID: 11569 RVA: 0x000BECA5 File Offset: 0x000BCEA5
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				this.ServerFixedUpdate();
			}
		}

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06002D32 RID: 11570 RVA: 0x000BECBA File Offset: 0x000BCEBA
		private bool bossShouldBeInvulnerable
		{
			get
			{
				return this.missionController.beaconsActive < this.missionController.beaconsToSpawnOnMap;
			}
		}

		// Token: 0x06002D33 RID: 11571 RVA: 0x000BECD4 File Offset: 0x000BCED4
		private void SetBossImmunity(bool newBossImmunity)
		{
			if (!this.bossInstanceBody)
			{
				return;
			}
			if (newBossImmunity == this.bossImmunity)
			{
				return;
			}
			this.bossImmunity = newBossImmunity;
			if (this.bossImmunity)
			{
				this.bossInstanceBody.AddBuff(BuffIndex.Immune);
				return;
			}
			EffectManager.SpawnEffect(GoldshoresBossfight.shieldRemovalEffectPrefab, new EffectData
			{
				origin = this.bossInstanceBody.coreTransform.position
			}, true);
			this.bossInstanceBody.RemoveBuff(BuffIndex.Immune);
		}

		// Token: 0x06002D34 RID: 11572 RVA: 0x000BED4C File Offset: 0x000BCF4C
		private void ExtinguishBeacons()
		{
			foreach (GameObject gameObject in this.missionController.beaconInstanceList)
			{
				gameObject.GetComponent<EntityStateMachine>().SetNextState(new NotReady());
			}
		}

		// Token: 0x06002D35 RID: 11573 RVA: 0x000BEDAC File Offset: 0x000BCFAC
		private void ServerFixedUpdate()
		{
			if (base.fixedAge >= GoldshoresBossfight.transitionDuration)
			{
				this.missionController.ExitTransitionIntoBossfight();
				if (!this.hasSpawnedBoss)
				{
					this.SpawnBoss();
				}
				else if (this.scriptedCombatEncounter.combatSquad.readOnlyMembersList.Count == 0)
				{
					this.outer.SetNextState(new Exit());
					if (this.serverCycleCount < 1)
					{
						Action action = GoldshoresBossfight.onOneCycleGoldTitanKill;
						if (action == null)
						{
							return;
						}
						action();
					}
					return;
				}
			}
			if (this.bossInstanceBody)
			{
				if (!this.bossImmunity)
				{
					if (this.bossInvulnerabilityStartTime.hasPassed)
					{
						this.ExtinguishBeacons();
						this.SetBossImmunity(true);
						this.serverCycleCount++;
						return;
					}
				}
				else if (this.missionController.beaconsActive >= this.missionController.beaconsToSpawnOnMap)
				{
					this.SetBossImmunity(false);
					this.bossInvulnerabilityStartTime = Run.FixedTimeStamp.now + GoldshoresBossfight.shieldRemovalDuration;
				}
			}
		}

		// Token: 0x06002D36 RID: 11574 RVA: 0x000BEE98 File Offset: 0x000BD098
		private void SpawnBoss()
		{
			if (this.hasSpawnedBoss)
			{
				return;
			}
			if (!this.scriptedCombatEncounter)
			{
				this.scriptedCombatEncounter = UnityEngine.Object.Instantiate<GameObject>(GoldshoresBossfight.combatEncounterPrefab).GetComponent<ScriptedCombatEncounter>();
				this.scriptedCombatEncounter.GetComponent<BossGroup>().dropPosition = this.missionController.bossSpawnPosition;
				NetworkServer.Spawn(this.scriptedCombatEncounter.gameObject);
			}
			this.scriptedCombatEncounter.BeginEncounter();
			this.hasSpawnedBoss = this.scriptedCombatEncounter.hasSpawnedServer;
			if (this.hasSpawnedBoss)
			{
				this.bossInstanceBody = this.scriptedCombatEncounter.combatSquad.readOnlyMembersList[0].GetBody();
				this.SetBossImmunity(true);
			}
		}

		// Token: 0x1400008D RID: 141
		// (add) Token: 0x06002D37 RID: 11575 RVA: 0x000BEF48 File Offset: 0x000BD148
		// (remove) Token: 0x06002D38 RID: 11576 RVA: 0x000BEF7C File Offset: 0x000BD17C
		public static event Action onOneCycleGoldTitanKill;

		// Token: 0x0400296E RID: 10606
		private GoldshoresMissionController missionController;

		// Token: 0x0400296F RID: 10607
		public static float shieldRemovalDuration;

		// Token: 0x04002970 RID: 10608
		public static GameObject shieldRemovalEffectPrefab;

		// Token: 0x04002971 RID: 10609
		public static GameObject shieldRegenerationEffectPrefab;

		// Token: 0x04002972 RID: 10610
		public static GameObject combatEncounterPrefab;

		// Token: 0x04002973 RID: 10611
		private static float transitionDuration = 3f;

		// Token: 0x04002974 RID: 10612
		private bool hasSpawnedBoss;

		// Token: 0x04002975 RID: 10613
		private CharacterBody bossInstanceBody;

		// Token: 0x04002976 RID: 10614
		private int serverCycleCount;

		// Token: 0x04002977 RID: 10615
		private Run.FixedTimeStamp bossInvulnerabilityStartTime;

		// Token: 0x04002978 RID: 10616
		private ScriptedCombatEncounter scriptedCombatEncounter;

		// Token: 0x04002979 RID: 10617
		private bool bossImmunity;
	}
}
