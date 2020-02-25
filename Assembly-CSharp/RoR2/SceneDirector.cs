using System;
using System.Collections.Generic;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000315 RID: 789
	[RequireComponent(typeof(DirectorCore))]
	public class SceneDirector : MonoBehaviour
	{
		// Token: 0x0600128C RID: 4748 RVA: 0x0004FA17 File Offset: 0x0004DC17
		private void Awake()
		{
			this.directorCore = base.GetComponent<DirectorCore>();
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x0004FA28 File Offset: 0x0004DC28
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
				float num = 0.5f + (float)Run.instance.participatingPlayerCount * 0.5f;
				ClassicStageInfo component = SceneInfo.instance.GetComponent<ClassicStageInfo>();
				if (component)
				{
					this.interactableCredit = (int)((float)component.sceneDirectorInteractibleCredits * num);
					if (component.bonusInteractibleCreditObjects != null)
					{
						for (int i = 0; i < component.bonusInteractibleCreditObjects.Length; i++)
						{
							ClassicStageInfo.BonusInteractibleCreditObject bonusInteractibleCreditObject = component.bonusInteractibleCreditObjects[i];
							if (bonusInteractibleCreditObject.objectThatGrantsPointsIfEnabled.activeSelf)
							{
								this.interactableCredit += bonusInteractibleCreditObject.points;
							}
						}
					}
					Debug.LogFormat("Spending {0} credits on interactables...", new object[]
					{
						this.interactableCredit
					});
					this.monsterCredit = (int)((float)component.sceneDirectorMonsterCredits * Run.instance.difficultyCoefficient);
				}
				Action<SceneDirector> action = SceneDirector.onPrePopulateSceneServer;
				if (action != null)
				{
					action(this);
				}
				this.PopulateScene();
				Action<SceneDirector> action2 = SceneDirector.onPostPopulateSceneServer;
				if (action2 == null)
				{
					return;
				}
				action2(this);
			}
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x0004FB40 File Offset: 0x0004DD40
		private void PlaceTeleporter()
		{
			if (!this.teleporterInstance && this.teleporterSpawnCard)
			{
				this.teleporterInstance = this.directorCore.TrySpawnObject(new DirectorSpawnRequest(this.teleporterSpawnCard, new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Random
				}, this.rng));
				Run.instance.OnServerTeleporterPlaced(this, this.teleporterInstance);
			}
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x0004FBA8 File Offset: 0x0004DDA8
		private static bool IsNodeSuitableForPod(NodeGraph nodeGraph, NodeGraph.NodeIndex nodeIndex)
		{
			NodeFlags nodeFlags;
			return nodeGraph.GetNodeFlags(nodeIndex, out nodeFlags) && (nodeFlags & NodeFlags.NoCeiling) != NodeFlags.None;
		}

		// Token: 0x06001290 RID: 4752 RVA: 0x0004FBC8 File Offset: 0x0004DDC8
		private void PlacePlayerSpawnsViaNodegraph()
		{
			bool usePod = Stage.instance.usePod;
			NodeGraph groundNodes = SceneInfo.instance.groundNodes;
			NodeFlags requiredFlags = NodeFlags.None;
			NodeFlags nodeFlags = NodeFlags.None;
			nodeFlags |= NodeFlags.NoCharacterSpawn;
			List<NodeGraph.NodeIndex> activeNodesForHullMaskWithFlagConditions = groundNodes.GetActiveNodesForHullMaskWithFlagConditions(HullMask.Golem, requiredFlags, nodeFlags);
			if (usePod)
			{
				int num = activeNodesForHullMaskWithFlagConditions.Count - 1;
				while (num >= 0 && activeNodesForHullMaskWithFlagConditions.Count > 1)
				{
					if (!SceneDirector.IsNodeSuitableForPod(groundNodes, activeNodesForHullMaskWithFlagConditions[num]))
					{
						activeNodesForHullMaskWithFlagConditions.RemoveAt(num);
					}
					num--;
				}
			}
			NodeGraph.NodeIndex nodeIndex;
			if (this.teleporterInstance)
			{
				Vector3 position = this.teleporterInstance.transform.position;
				List<SceneDirector.NodeDistanceSqrPair> list = new List<SceneDirector.NodeDistanceSqrPair>();
				for (int i = 0; i < activeNodesForHullMaskWithFlagConditions.Count; i++)
				{
					Vector3 b2;
					groundNodes.GetNodePosition(activeNodesForHullMaskWithFlagConditions[i], out b2);
					list.Add(new SceneDirector.NodeDistanceSqrPair
					{
						nodeIndex = activeNodesForHullMaskWithFlagConditions[i],
						distanceSqr = (position - b2).sqrMagnitude
					});
				}
				list.Sort((SceneDirector.NodeDistanceSqrPair a, SceneDirector.NodeDistanceSqrPair b) => a.distanceSqr.CompareTo(b.distanceSqr));
				int index = this.rng.RangeInt(list.Count * 3 / 4, list.Count);
				nodeIndex = list[index].nodeIndex;
			}
			else
			{
				nodeIndex = this.rng.NextElementUniform<NodeGraph.NodeIndex>(activeNodesForHullMaskWithFlagConditions);
			}
			NodeGraphSpider nodeGraphSpider = new NodeGraphSpider(groundNodes, HullMask.Human);
			nodeGraphSpider.AddNodeForNextStep(nodeIndex);
			while (nodeGraphSpider.PerformStep())
			{
				List<NodeGraphSpider.StepInfo> collectedSteps = nodeGraphSpider.collectedSteps;
				if (usePod)
				{
					for (int j = collectedSteps.Count - 1; j >= 0; j--)
					{
						if (!SceneDirector.IsNodeSuitableForPod(groundNodes, collectedSteps[j].node))
						{
							collectedSteps.RemoveAt(j);
						}
					}
				}
				if (collectedSteps.Count >= RoR2Application.maxPlayers)
				{
					break;
				}
			}
			List<NodeGraphSpider.StepInfo> collectedSteps2 = nodeGraphSpider.collectedSteps;
			Util.ShuffleList<NodeGraphSpider.StepInfo>(collectedSteps2, Run.instance.stageRng);
			int num2 = Math.Min(nodeGraphSpider.collectedSteps.Count, RoR2Application.maxPlayers);
			for (int k = 0; k < num2; k++)
			{
				SpawnPoint.AddSpawnPoint(groundNodes, collectedSteps2[k].node, this.rng);
			}
		}

		// Token: 0x06001291 RID: 4753 RVA: 0x0004FDFC File Offset: 0x0004DFFC
		private void RemoveAllExistingSpawnPoints()
		{
			List<SpawnPoint> list = new List<SpawnPoint>(SpawnPoint.readOnlyInstancesList);
			for (int i = 0; i < list.Count; i++)
			{
				UnityEngine.Object.Destroy(list[i].gameObject);
			}
		}

		// Token: 0x06001292 RID: 4754 RVA: 0x0004FE38 File Offset: 0x0004E038
		private void CullExistingSpawnPoints()
		{
			List<SpawnPoint> list = new List<SpawnPoint>(SpawnPoint.readOnlyInstancesList);
			if (this.teleporterInstance)
			{
				Vector3 teleporterPosition = this.teleporterInstance.transform.position;
				list.Sort((SpawnPoint a, SpawnPoint b) => (teleporterPosition - a.transform.position).sqrMagnitude.CompareTo((teleporterPosition - b.transform.position).sqrMagnitude));
				Debug.Log("reorder list");
				for (int i = list.Count; i >= 0; i--)
				{
					if (i < list.Count - RoR2Application.maxPlayers)
					{
						UnityEngine.Object.Destroy(list[i].gameObject);
					}
				}
			}
		}

		// Token: 0x06001293 RID: 4755 RVA: 0x0004FEC6 File Offset: 0x0004E0C6
		private void DefaultPlayerSpawnPointGenerator()
		{
			if (SpawnPoint.readOnlyInstancesList.Count == 0 || (Stage.instance && !Stage.instance.usePod))
			{
				this.RemoveAllExistingSpawnPoints();
				this.PlacePlayerSpawnsViaNodegraph();
				return;
			}
			this.CullExistingSpawnPoints();
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x0004FF00 File Offset: 0x0004E100
		private void PopulateScene()
		{
			ClassicStageInfo component = SceneInfo.instance.GetComponent<ClassicStageInfo>();
			this.PlaceTeleporter();
			Action action = new Action(this.DefaultPlayerSpawnPointGenerator);
			SceneDirector.GenerateSpawnPointsDelegate generateSpawnPointsDelegate = SceneDirector.onPreGeneratePlayerSpawnPointsServer;
			if (generateSpawnPointsDelegate != null)
			{
				generateSpawnPointsDelegate(this, ref action);
			}
			if (action != null)
			{
				action();
			}
			Run.instance.OnPlayerSpawnPointsPlaced(this);
			while (this.interactableCredit > 0)
			{
				DirectorCard directorCard = this.SelectCard(component.interactableSelection, this.interactableCredit);
				if (directorCard == null)
				{
					break;
				}
				if (directorCard.CardIsValid())
				{
					this.interactableCredit -= directorCard.cost;
					if (Run.instance)
					{
						int i = 0;
						while (i < 10)
						{
							DirectorPlacementRule placementRule = new DirectorPlacementRule
							{
								placementMode = DirectorPlacementRule.PlacementMode.Random
							};
							GameObject gameObject = this.directorCore.TrySpawnObject(new DirectorSpawnRequest(directorCard.spawnCard, placementRule, this.rng));
							if (gameObject)
							{
								PurchaseInteraction component2 = gameObject.GetComponent<PurchaseInteraction>();
								if (component2 && component2.costType == CostTypeIndex.Money)
								{
									component2.Networkcost = Run.instance.GetDifficultyScaledCost(component2.cost);
									break;
								}
								break;
							}
							else
							{
								i++;
							}
						}
					}
				}
			}
			if (Run.instance && Run.instance.stageClearCount == 0)
			{
				this.monsterCredit = 0;
			}
			int num = 0;
			while (this.monsterCredit > 0 && num < 40)
			{
				DirectorCard directorCard2 = this.SelectCard(component.monsterSelection, this.monsterCredit);
				if (directorCard2 == null)
				{
					break;
				}
				if (directorCard2.CardIsValid())
				{
					this.monsterCredit -= directorCard2.cost;
					int j = 0;
					while (j < 10)
					{
						DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(directorCard2.spawnCard, new DirectorPlacementRule
						{
							placementMode = DirectorPlacementRule.PlacementMode.Random
						}, this.rng);
						directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Monster);
						GameObject gameObject2 = this.directorCore.TrySpawnObject(directorSpawnRequest);
						if (gameObject2)
						{
							num++;
							CharacterMaster component3 = gameObject2.GetComponent<CharacterMaster>();
							if (component3)
							{
								GameObject bodyObject = component3.GetBodyObject();
								if (bodyObject)
								{
									DeathRewards component4 = bodyObject.GetComponent<DeathRewards>();
									if (component4)
									{
										component4.expReward = (uint)((float)directorCard2.cost * this.expRewardCoefficient * Run.instance.difficultyCoefficient);
										component4.goldReward = (uint)((float)directorCard2.cost * this.expRewardCoefficient * 2f * Run.instance.difficultyCoefficient);
									}
									foreach (EntityStateMachine entityStateMachine in bodyObject.GetComponents<EntityStateMachine>())
									{
										entityStateMachine.initialStateType = entityStateMachine.mainStateType;
									}
								}
								num++;
								break;
							}
							break;
						}
						else
						{
							j++;
						}
					}
				}
			}
			Xoroshiro128Plus xoroshiro128Plus = new Xoroshiro128Plus(this.rng.nextUlong);
			if (SceneInfo.instance.countsAsStage)
			{
				int num2 = 0;
				foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
				{
					num2 += characterMaster.inventory.GetItemCount(ItemIndex.TreasureCache);
				}
				if (num2 > 0)
				{
					GameObject gameObject3 = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscLockbox"), new DirectorPlacementRule
					{
						placementMode = DirectorPlacementRule.PlacementMode.Random
					}, xoroshiro128Plus));
					if (gameObject3)
					{
						ChestBehavior component5 = gameObject3.GetComponent<ChestBehavior>();
						if (component5)
						{
							component5.tier2Chance *= (float)num2;
							component5.tier3Chance *= Mathf.Pow((float)num2, 2f);
						}
					}
				}
			}
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x0005029C File Offset: 0x0004E49C
		private DirectorCard SelectCard(WeightedSelection<DirectorCard> deck, int maxCost)
		{
			SceneDirector.cardSelector.Clear();
			int i = 0;
			int count = deck.Count;
			while (i < count)
			{
				WeightedSelection<DirectorCard>.ChoiceInfo choice = deck.GetChoice(i);
				if (choice.value.cost <= maxCost)
				{
					SceneDirector.cardSelector.AddChoice(choice);
				}
				i++;
			}
			if (SceneDirector.cardSelector.Count == 0)
			{
				return null;
			}
			return SceneDirector.cardSelector.Evaluate(this.rng.nextNormalizedFloat);
		}

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06001296 RID: 4758 RVA: 0x0005030C File Offset: 0x0004E50C
		// (remove) Token: 0x06001297 RID: 4759 RVA: 0x00050340 File Offset: 0x0004E540
		public static event SceneDirector.GenerateSpawnPointsDelegate onPreGeneratePlayerSpawnPointsServer;

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06001298 RID: 4760 RVA: 0x00050374 File Offset: 0x0004E574
		// (remove) Token: 0x06001299 RID: 4761 RVA: 0x000503A8 File Offset: 0x0004E5A8
		public static event Action<SceneDirector> onPrePopulateSceneServer;

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x0600129A RID: 4762 RVA: 0x000503DC File Offset: 0x0004E5DC
		// (remove) Token: 0x0600129B RID: 4763 RVA: 0x00050410 File Offset: 0x0004E610
		public static event Action<SceneDirector> onPostPopulateSceneServer;

		// Token: 0x04001173 RID: 4467
		private DirectorCore directorCore;

		// Token: 0x04001174 RID: 4468
		public SpawnCard teleporterSpawnCard;

		// Token: 0x04001175 RID: 4469
		public float expRewardCoefficient;

		// Token: 0x04001176 RID: 4470
		private int interactableCredit;

		// Token: 0x04001177 RID: 4471
		private int monsterCredit;

		// Token: 0x04001178 RID: 4472
		public GameObject teleporterInstance;

		// Token: 0x04001179 RID: 4473
		private Xoroshiro128Plus rng;

		// Token: 0x0400117A RID: 4474
		private static readonly WeightedSelection<DirectorCard> cardSelector = new WeightedSelection<DirectorCard>(8);

		// Token: 0x02000316 RID: 790
		private struct NodeDistanceSqrPair
		{
			// Token: 0x0400117E RID: 4478
			public NodeGraph.NodeIndex nodeIndex;

			// Token: 0x0400117F RID: 4479
			public float distanceSqr;
		}

		// Token: 0x02000317 RID: 791
		// (Invoke) Token: 0x0600129F RID: 4767
		public delegate void GenerateSpawnPointsDelegate(SceneDirector sceneDirector, ref Action generationMethod);
	}
}
