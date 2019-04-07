using System;
using System.Collections.Generic;
using System.Globalization;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000420 RID: 1056
	public class WeeklyRun : Run
	{
		// Token: 0x06001776 RID: 6006 RVA: 0x0006F320 File Offset: 0x0006D520
		public static uint GetCurrentSeedCycle()
		{
			return (uint)((WeeklyRun.now - WeeklyRun.startDate).Days / 3);
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x0006F348 File Offset: 0x0006D548
		public static DateTime GetSeedCycleStartDateTime(uint seedCycle)
		{
			return WeeklyRun.startDate.AddDays(seedCycle * 3u);
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06001778 RID: 6008 RVA: 0x0006F367 File Offset: 0x0006D567
		public static DateTime now
		{
			get
			{
				return Util.UnixTimeStampToDateTimeUtc(Client.Instance.Utils.GetServerRealTime());
			}
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x0006F37D File Offset: 0x0006D57D
		protected new void Start()
		{
			base.Start();
			this.bossAffixRng = new Xoroshiro128Plus(this.runRNG.nextUlong);
			if (NetworkServer.active)
			{
				this.NetworkserverSeedCycle = WeeklyRun.GetCurrentSeedCycle();
			}
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x0006F3B0 File Offset: 0x0006D5B0
		protected override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
			if (TeleporterInteraction.instance)
			{
				bool flag = this.crystalsRequiredToKill > this.crystalsKilled;
				if (flag != TeleporterInteraction.instance.locked)
				{
					if (flag)
					{
						if (NetworkServer.active)
						{
							TeleporterInteraction.instance.Networklocked = true;
							return;
						}
					}
					else
					{
						if (NetworkServer.active)
						{
							TeleporterInteraction.instance.Networklocked = false;
						}
						ChildLocator component = TeleporterInteraction.instance.GetComponent<ModelLocator>().modelTransform.GetComponent<ChildLocator>();
						if (component)
						{
							Transform transform = component.FindChild("TimeCrystalBeaconBlocker");
							EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/TimeCrystalDeath"), new EffectData
							{
								origin = transform.transform.position
							}, false);
							transform.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x0006F479 File Offset: 0x0006D679
		protected override void OverrideSeed()
		{
			this.seed = (ulong)WeeklyRun.GetCurrentSeedCycle();
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x00004507 File Offset: 0x00002707
		public override void HandlePlayerFirstEntryAnimation(CharacterBody body, Vector3 spawnPosition, Quaternion spawnRotation)
		{
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x0006F487 File Offset: 0x0006D687
		public override void AdvanceStage(string nextSceneName)
		{
			if (this.stageClearCount == 1 && SceneInfo.instance.countsAsStage)
			{
				base.BeginGameOver(GameResultType.Won);
				return;
			}
			base.AdvanceStage(nextSceneName);
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x0006F4AD File Offset: 0x0006D6AD
		public override void OnClientGameOver(RunReport runReport)
		{
			base.OnClientGameOver(runReport);
			this.ClientSubmitLeaderboardScore(runReport);
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x0006F4C0 File Offset: 0x0006D6C0
		public override void OnServerBossAdded(BossGroup bossGroup, CharacterMaster characterMaster)
		{
			base.OnServerBossAdded(bossGroup, characterMaster);
			if (this.stageClearCount >= 1)
			{
				if (characterMaster.inventory.GetEquipmentIndex() == EquipmentIndex.None)
				{
					characterMaster.inventory.SetEquipmentIndex(WeeklyRun.affixes[this.bossAffixRng.RangeInt(0, WeeklyRun.affixes.Length)]);
				}
				characterMaster.inventory.GiveItem(ItemIndex.BoostHp, 5);
				characterMaster.inventory.GiveItem(ItemIndex.BoostDamage, 1);
			}
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x0006F52C File Offset: 0x0006D72C
		public override void OnServerBossKilled(bool bossGroupDefeated)
		{
			base.OnServerBossKilled(bossGroupDefeated);
			if (TeleporterInteraction.instance && bossGroupDefeated)
			{
				TeleporterInteraction.instance.remainingChargeTimer = 0f;
			}
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x0006F552 File Offset: 0x0006D752
		public override GameObject GetTeleportEffectPrefab(GameObject objectToTeleport)
		{
			return Resources.Load<GameObject>("Prefabs/Effects/TeleportOutCrystalBoom");
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06001782 RID: 6018 RVA: 0x0006F55E File Offset: 0x0006D75E
		public uint crystalsKilled
		{
			get
			{
				return (uint)((ulong)this.crystalCount - (ulong)((long)this.crystalActiveList.Count));
			}
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x0006F578 File Offset: 0x0006D778
		public override void OnServerTeleporterPlaced(SceneDirector sceneDirector, GameObject teleporter)
		{
			base.OnServerTeleporterPlaced(sceneDirector, teleporter);
			DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule();
			directorPlacementRule.placementMode = DirectorPlacementRule.PlacementMode.Random;
			int num = 0;
			while ((long)num < (long)((ulong)this.crystalCount))
			{
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(this.crystalSpawnCard, directorPlacementRule, this.stageRng);
				if (gameObject)
				{
					DeathRewards component3 = gameObject.GetComponent<DeathRewards>();
					if (component3)
					{
						component3.goldReward = this.crystalRewardValue;
					}
				}
				this.crystalActiveList.Add(OnDestroyCallback.AddCallback(gameObject, delegate(OnDestroyCallback component)
				{
					this.crystalActiveList.Remove(component);
				}));
				num++;
			}
			if (TeleporterInteraction.instance)
			{
				ChildLocator component2 = TeleporterInteraction.instance.GetComponent<ModelLocator>().modelTransform.GetComponent<ChildLocator>();
				if (component2)
				{
					component2.FindChild("TimeCrystalProps").gameObject.SetActive(true);
					component2.FindChild("TimeCrystalBeaconBlocker").gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x0006F660 File Offset: 0x0006D860
		public override void OnPlayerSpawnPointsPlaced(SceneDirector sceneDirector)
		{
			if (this.stageClearCount == 0)
			{
				SpawnPoint spawnPoint = SpawnPoint.readOnlyInstancesList[0];
				if (spawnPoint)
				{
					float num = 360f / this.equipmentBarrelCount;
					int num2 = 0;
					while ((long)num2 < (long)((ulong)this.equipmentBarrelCount))
					{
						Vector3 b = Quaternion.AngleAxis(num * (float)num2, Vector3.up) * (Vector3.forward * this.equipmentBarrelRadius);
						DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule();
						directorPlacementRule.minDistance = 0f;
						directorPlacementRule.maxDistance = 3f;
						directorPlacementRule.placementMode = DirectorPlacementRule.PlacementMode.NearestNode;
						directorPlacementRule.position = spawnPoint.transform.position + b;
						DirectorCore.instance.TrySpawnObject(this.equipmentBarrelSpawnCard, directorPlacementRule, this.stageRng);
						num2++;
					}
				}
			}
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x0006F733 File Offset: 0x0006D933
		public static string GetLeaderboardName(int playerCount, uint seedCycle)
		{
			return string.Format(CultureInfo.InvariantCulture, "weekly{0}p{1}", playerCount, seedCycle);
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x0006F750 File Offset: 0x0006D950
		protected void ClientSubmitLeaderboardScore(RunReport runReport)
		{
			if (runReport.gameResultType != GameResultType.Won)
			{
				return;
			}
			bool flag = false;
			using (IEnumerator<NetworkUser> enumerator = NetworkUser.readOnlyLocalPlayersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isParticipating)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			int num = PlayerCharacterMasterController.instances.Count;
			if (num <= 0)
			{
				return;
			}
			if (num >= 3)
			{
				if (num > 4)
				{
					return;
				}
				num = 4;
			}
			string name = WeeklyRun.GetLeaderboardName(num, this.serverSeedCycle);
			int[] subScores = new int[64];
			GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(NetworkUser.readOnlyLocalPlayersList[0].bodyIndexPreference);
			if (!bodyPrefab)
			{
				return;
			}
			SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab);
			if (survivorDef == null)
			{
				return;
			}
			subScores[1] = (int)survivorDef.survivorIndex;
			Leaderboard leaderboard = Client.Instance.GetLeaderboard(name, Client.LeaderboardSortMethod.Ascending, Client.LeaderboardDisplayType.TimeMilliSeconds);
			leaderboard.OnBoardInformation = delegate()
			{
				leaderboard.AddScore(true, (int)Math.Ceiling((double)runReport.snapshotTime.t * 1000.0), subScores);
			};
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x0006F864 File Offset: 0x0006DA64
		public override void OverrideRuleChoices(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude)
		{
			base.OverrideRuleChoices(mustInclude, mustExclude);
			base.ForceChoice(mustInclude, mustExclude, "Difficulty.Normal");
			base.ForceChoice(mustInclude, mustExclude, "Misc.StartingMoney.50");
			base.ForceChoice(mustInclude, mustExclude, "Misc.StageOrder.Random");
			base.ForceChoice(mustInclude, mustExclude, "Misc.KeepMoneyBetweenStages.Off");
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				RuleDef ruleDef = RuleCatalog.FindRuleDef(artifactIndex.ToString());
				RuleChoiceDef ruleChoiceDef = (ruleDef != null) ? ruleDef.FindChoice("Off") : null;
				if (ruleChoiceDef != null)
				{
					base.ForceChoice(mustInclude, mustExclude, ruleChoiceDef);
				}
			}
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				RuleDef ruleDef2 = RuleCatalog.FindRuleDef("Items." + itemIndex.ToString());
				RuleChoiceDef ruleChoiceDef2 = (ruleDef2 != null) ? ruleDef2.FindChoice("On") : null;
				if (ruleChoiceDef2 != null)
				{
					base.ForceChoice(mustInclude, mustExclude, ruleChoiceDef2);
				}
			}
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				RuleDef ruleDef3 = RuleCatalog.FindRuleDef("Equipment." + equipmentIndex.ToString());
				RuleChoiceDef ruleChoiceDef3 = (ruleDef3 != null) ? ruleDef3.FindChoice("On") : null;
				if (ruleChoiceDef3 != null)
				{
					base.ForceChoice(mustInclude, mustExclude, ruleChoiceDef3);
				}
			}
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override bool IsUnlockableUnlocked(string unlockableName)
		{
			return true;
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x0000A1ED File Offset: 0x000083ED
		public override bool CanUnlockableBeGrantedThisRun(string unlockableName)
		{
			return false;
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override bool DoesEveryoneHaveThisUnlockableUnlocked(string unlockableName)
		{
			return true;
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x0006F97E File Offset: 0x0006DB7E
		protected override void HandlePostRunDestination()
		{
			Console.instance.SubmitCmd(null, "transition_command \"disconnect\";", false);
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06001790 RID: 6032 RVA: 0x0006FA08 File Offset: 0x0006DC08
		// (set) Token: 0x06001791 RID: 6033 RVA: 0x0006FA1B File Offset: 0x0006DC1B
		public uint NetworkserverSeedCycle
		{
			get
			{
				return this.serverSeedCycle;
			}
			set
			{
				base.SetSyncVar<uint>(value, ref this.serverSeedCycle, 64u);
			}
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x0006FA30 File Offset: 0x0006DC30
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool flag = base.OnSerialize(writer, forceAll);
			if (forceAll)
			{
				writer.WritePackedUInt32(this.serverSeedCycle);
				return true;
			}
			bool flag2 = false;
			if ((base.syncVarDirtyBits & 64u) != 0u)
			{
				if (!flag2)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag2 = true;
				}
				writer.WritePackedUInt32(this.serverSeedCycle);
			}
			if (!flag2)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag2 || flag;
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x0006FAA8 File Offset: 0x0006DCA8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
			if (initialState)
			{
				this.serverSeedCycle = reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 64) != 0)
			{
				this.serverSeedCycle = reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04001AA2 RID: 6818
		private Xoroshiro128Plus bossAffixRng;

		// Token: 0x04001AA3 RID: 6819
		public static readonly DateTime startDate = new DateTime(2018, 8, 27, 0, 0, 0, 0, DateTimeKind.Utc);

		// Token: 0x04001AA4 RID: 6820
		public const int cycleLength = 3;

		// Token: 0x04001AA5 RID: 6821
		private string leaderboardName;

		// Token: 0x04001AA6 RID: 6822
		[SyncVar]
		private uint serverSeedCycle;

		// Token: 0x04001AA7 RID: 6823
		private static readonly EquipmentIndex[] affixes = new EquipmentIndex[]
		{
			EquipmentIndex.AffixBlue,
			EquipmentIndex.AffixRed
		};

		// Token: 0x04001AA8 RID: 6824
		public SpawnCard crystalSpawnCard;

		// Token: 0x04001AA9 RID: 6825
		public uint crystalCount = 3u;

		// Token: 0x04001AAA RID: 6826
		public uint crystalRewardValue = 50u;

		// Token: 0x04001AAB RID: 6827
		public uint crystalsRequiredToKill = 3u;

		// Token: 0x04001AAC RID: 6828
		private List<OnDestroyCallback> crystalActiveList = new List<OnDestroyCallback>();

		// Token: 0x04001AAD RID: 6829
		public SpawnCard equipmentBarrelSpawnCard;

		// Token: 0x04001AAE RID: 6830
		public uint equipmentBarrelCount = 3u;

		// Token: 0x04001AAF RID: 6831
		public float equipmentBarrelRadius = 10f;
	}
}
