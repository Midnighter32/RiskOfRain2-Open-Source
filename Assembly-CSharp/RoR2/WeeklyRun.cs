using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200037C RID: 892
	public class WeeklyRun : Run
	{
		// Token: 0x060015AE RID: 5550 RVA: 0x0005C61C File Offset: 0x0005A81C
		public static uint GetCurrentSeedCycle()
		{
			return (uint)((WeeklyRun.now - WeeklyRun.startDate).Days / 3);
		}

		// Token: 0x060015AF RID: 5551 RVA: 0x0005C644 File Offset: 0x0005A844
		public static DateTime GetSeedCycleStartDateTime(uint seedCycle)
		{
			return WeeklyRun.startDate.AddDays(seedCycle * 3U);
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x060015B0 RID: 5552 RVA: 0x0005C663 File Offset: 0x0005A863
		public static DateTime now
		{
			get
			{
				return Util.UnixTimeStampToDateTimeUtc(Client.Instance.Utils.GetServerRealTime());
			}
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x0005C679 File Offset: 0x0005A879
		protected new void Start()
		{
			base.Start();
			this.bossAffixRng = new Xoroshiro128Plus(this.runRNG.nextUlong);
			if (NetworkServer.active)
			{
				this.NetworkserverSeedCycle = WeeklyRun.GetCurrentSeedCycle();
			}
		}

		// Token: 0x060015B2 RID: 5554 RVA: 0x0005C6AC File Offset: 0x0005A8AC
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
							EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/TimeCrystalDeath"), new EffectData
							{
								origin = transform.transform.position
							}, false);
							transform.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x0005C770 File Offset: 0x0005A970
		protected override void OverrideSeed()
		{
			this.seed = (ulong)WeeklyRun.GetCurrentSeedCycle();
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x0000409B File Offset: 0x0000229B
		public override void HandlePlayerFirstEntryAnimation(CharacterBody body, Vector3 spawnPosition, Quaternion spawnRotation)
		{
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x0005C77E File Offset: 0x0005A97E
		public override void AdvanceStage(SceneDef nextScene)
		{
			if (this.stageClearCount == 1 && SceneInfo.instance.countsAsStage)
			{
				base.BeginGameOver(GameResultType.Won);
				return;
			}
			base.AdvanceStage(nextScene);
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x0005C7A4 File Offset: 0x0005A9A4
		public override void OnClientGameOver(RunReport runReport)
		{
			base.OnClientGameOver(runReport);
			this.ClientSubmitLeaderboardScore(runReport);
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x0005C7B4 File Offset: 0x0005A9B4
		public unsafe override void OnServerBossAdded(BossGroup bossGroup, CharacterMaster characterMaster)
		{
			base.OnServerBossAdded(bossGroup, characterMaster);
			if (this.stageClearCount >= 1)
			{
				if (characterMaster.inventory.GetEquipmentIndex() == EquipmentIndex.None)
				{
					characterMaster.inventory.SetEquipmentIndex(*this.bossAffixRng.NextElementUniform<EquipmentIndex>(WeeklyRun.affixes));
				}
				characterMaster.inventory.GiveItem(ItemIndex.BoostHp, 5);
				characterMaster.inventory.GiveItem(ItemIndex.BoostDamage, 1);
			}
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x0005C818 File Offset: 0x0005AA18
		public override void OnServerBossDefeated(BossGroup bossGroup)
		{
			base.OnServerBossDefeated(bossGroup);
			if (TeleporterInteraction.instance)
			{
				TeleporterInteraction.instance.remainingChargeTimer = 0f;
			}
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x0005C83C File Offset: 0x0005AA3C
		public override GameObject GetTeleportEffectPrefab(GameObject objectToTeleport)
		{
			return Resources.Load<GameObject>("Prefabs/Effects/TeleportOutCrystalBoom");
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x060015BA RID: 5562 RVA: 0x0005C848 File Offset: 0x0005AA48
		public uint crystalsKilled
		{
			get
			{
				return (uint)((ulong)this.crystalCount - (ulong)((long)this.crystalActiveList.Count));
			}
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x0005C860 File Offset: 0x0005AA60
		public override void OnServerTeleporterPlaced(SceneDirector sceneDirector, GameObject teleporter)
		{
			base.OnServerTeleporterPlaced(sceneDirector, teleporter);
			DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule();
			directorPlacementRule.placementMode = DirectorPlacementRule.PlacementMode.Random;
			int num = 0;
			while ((long)num < (long)((ulong)this.crystalCount))
			{
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(this.crystalSpawnCard, directorPlacementRule, this.stageRng));
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

		// Token: 0x060015BC RID: 5564 RVA: 0x0005C94C File Offset: 0x0005AB4C
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
						DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(this.equipmentBarrelSpawnCard, directorPlacementRule, this.stageRng));
						num2++;
					}
				}
			}
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x0005CA24 File Offset: 0x0005AC24
		public static string GetLeaderboardName(int playerCount, uint seedCycle)
		{
			if (RoR2Application.sessionCheatsEnabled)
			{
				return null;
			}
			return string.Format(CultureInfo.InvariantCulture, "weekly{0}p{1}", playerCount, seedCycle);
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x0005CA4C File Offset: 0x0005AC4C
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
			string text = WeeklyRun.GetLeaderboardName(num, this.serverSeedCycle);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
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
			Leaderboard leaderboard = Client.Instance.GetLeaderboard(text, Client.LeaderboardSortMethod.Ascending, Client.LeaderboardDisplayType.TimeMilliSeconds);
			leaderboard.OnBoardInformation = delegate()
			{
				leaderboard.AddScore(true, (int)Math.Ceiling((double)runReport.runStopwatchValue * 1000.0), subScores);
			};
		}

		// Token: 0x060015BF RID: 5567 RVA: 0x0005CB68 File Offset: 0x0005AD68
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
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
				RuleDef ruleDef2 = RuleCatalog.FindRuleDef("Items." + itemDef.name);
				RuleChoiceDef ruleChoiceDef2 = (ruleDef2 != null) ? ruleDef2.FindChoice("On") : null;
				if (ruleChoiceDef2 != null)
				{
					base.ForceChoice(mustInclude, mustExclude, ruleChoiceDef2);
				}
				itemIndex++;
			}
			EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile;
			EquipmentIndex equipmentCount = (EquipmentIndex)EquipmentCatalog.equipmentCount;
			while (equipmentIndex < equipmentCount)
			{
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
				RuleDef ruleDef3 = RuleCatalog.FindRuleDef("Equipment." + equipmentDef.name);
				RuleChoiceDef ruleChoiceDef3 = (ruleDef3 != null) ? ruleDef3.FindChoice("On") : null;
				if (ruleChoiceDef3 != null)
				{
					base.ForceChoice(mustInclude, mustExclude, ruleChoiceDef3);
				}
				equipmentIndex++;
			}
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool IsUnlockableUnlocked(string unlockableName)
		{
			return true;
		}

		// Token: 0x060015C1 RID: 5569 RVA: 0x0000AC89 File Offset: 0x00008E89
		public override bool CanUnlockableBeGrantedThisRun(string unlockableName)
		{
			return false;
		}

		// Token: 0x060015C2 RID: 5570 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool DoesEveryoneHaveThisUnlockableUnlocked(string unlockableName)
		{
			return true;
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x0005CC96 File Offset: 0x0005AE96
		protected override void HandlePostRunDestination()
		{
			Console.instance.SubmitCmd(null, "transition_command \"disconnect\";", false);
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x0005CCA9 File Offset: 0x0005AEA9
		protected override bool ShouldUpdateRunStopwatch()
		{
			return base.livingPlayerCount > 0;
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool ShouldAllowNonChampionBossSpawn()
		{
			return true;
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x060015CA RID: 5578 RVA: 0x0005CD2C File Offset: 0x0005AF2C
		// (set) Token: 0x060015CB RID: 5579 RVA: 0x0005CD3F File Offset: 0x0005AF3F
		public uint NetworkserverSeedCycle
		{
			get
			{
				return this.serverSeedCycle;
			}
			[param: In]
			set
			{
				base.SetSyncVar<uint>(value, ref this.serverSeedCycle, 128U);
			}
		}

		// Token: 0x060015CC RID: 5580 RVA: 0x0005CD54 File Offset: 0x0005AF54
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool flag = base.OnSerialize(writer, forceAll);
			if (forceAll)
			{
				writer.WritePackedUInt32(this.serverSeedCycle);
				return true;
			}
			bool flag2 = false;
			if ((base.syncVarDirtyBits & 128U) != 0U)
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

		// Token: 0x060015CD RID: 5581 RVA: 0x0005CDCC File Offset: 0x0005AFCC
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
			if (initialState)
			{
				this.serverSeedCycle = reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 128) != 0)
			{
				this.serverSeedCycle = reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04001438 RID: 5176
		private Xoroshiro128Plus bossAffixRng;

		// Token: 0x04001439 RID: 5177
		public static readonly DateTime startDate = new DateTime(2018, 8, 27, 0, 0, 0, 0, DateTimeKind.Utc);

		// Token: 0x0400143A RID: 5178
		public const int cycleLength = 3;

		// Token: 0x0400143B RID: 5179
		private string leaderboardName;

		// Token: 0x0400143C RID: 5180
		[SyncVar]
		private uint serverSeedCycle;

		// Token: 0x0400143D RID: 5181
		private static readonly EquipmentIndex[] affixes = new EquipmentIndex[]
		{
			EquipmentIndex.AffixBlue,
			EquipmentIndex.AffixRed
		};

		// Token: 0x0400143E RID: 5182
		public SpawnCard crystalSpawnCard;

		// Token: 0x0400143F RID: 5183
		public uint crystalCount = 3U;

		// Token: 0x04001440 RID: 5184
		public uint crystalRewardValue = 50U;

		// Token: 0x04001441 RID: 5185
		public uint crystalsRequiredToKill = 3U;

		// Token: 0x04001442 RID: 5186
		private List<OnDestroyCallback> crystalActiveList = new List<OnDestroyCallback>();

		// Token: 0x04001443 RID: 5187
		public SpawnCard equipmentBarrelSpawnCard;

		// Token: 0x04001444 RID: 5188
		public uint equipmentBarrelCount = 3U;

		// Token: 0x04001445 RID: 5189
		public float equipmentBarrelRadius = 10f;
	}
}
