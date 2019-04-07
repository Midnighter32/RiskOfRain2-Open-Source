using System;
using System.Collections.Generic;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003C8 RID: 968
	[RequireComponent(typeof(DirectorCore))]
	public class SceneDirector : MonoBehaviour
	{
		// Token: 0x0600150A RID: 5386 RVA: 0x00064DCB File Offset: 0x00062FCB
		private void Awake()
		{
			this.directorCore = base.GetComponent<DirectorCore>();
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x00064DDC File Offset: 0x00062FDC
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
					Debug.LogFormat("Spending {0} credits on interactables...", new object[]
					{
						this.interactableCredit
					});
					this.monsterCredit = (int)((float)component.sceneDirectorMonsterCredits * Run.instance.difficultyCoefficient);
				}
				this.PopulateScene();
			}
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x00064E88 File Offset: 0x00063088
		private void PlaceTeleporter()
		{
			if (!this.teleporterInstance && this.teleporterSpawnCard)
			{
				this.teleporterInstance = this.directorCore.TrySpawnObject(this.teleporterSpawnCard, new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Random
				}, this.rng);
				Run.instance.OnServerTeleporterPlaced(this, this.teleporterInstance);
			}
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x00064EEC File Offset: 0x000630EC
		private static bool IsNodeSuitableForPod(NodeGraph nodeGraph, NodeGraph.NodeIndex nodeIndex)
		{
			NodeFlags nodeFlags;
			return nodeGraph.GetNodeFlags(nodeIndex, out nodeFlags) && (nodeFlags & NodeFlags.NoCeiling) != NodeFlags.None;
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00064F0C File Offset: 0x0006310C
		private void PlacePlayerSpawnsViaNodegraph()
		{
			bool usePod = Stage.instance.usePod;
			NodeGraph groundNodes = SceneInfo.instance.groundNodes;
			List<NodeGraph.NodeIndex> activeNodesForHullMask = groundNodes.GetActiveNodesForHullMask(HullMask.Golem);
			if (usePod)
			{
				for (int i = activeNodesForHullMask.Count - 1; i >= 0; i--)
				{
					if (!SceneDirector.IsNodeSuitableForPod(groundNodes, activeNodesForHullMask[i]))
					{
						activeNodesForHullMask.RemoveAt(i);
					}
				}
			}
			NodeGraph.NodeIndex nodeIndex;
			if (this.teleporterInstance)
			{
				Vector3 position = this.teleporterInstance.transform.position;
				List<SceneDirector.NodeDistanceSqrPair> list = new List<SceneDirector.NodeDistanceSqrPair>();
				for (int j = 0; j < activeNodesForHullMask.Count; j++)
				{
					Vector3 b2;
					groundNodes.GetNodePosition(activeNodesForHullMask[j], out b2);
					list.Add(new SceneDirector.NodeDistanceSqrPair
					{
						nodeIndex = activeNodesForHullMask[j],
						distanceSqr = (position - b2).sqrMagnitude
					});
				}
				list.Sort((SceneDirector.NodeDistanceSqrPair a, SceneDirector.NodeDistanceSqrPair b) => a.distanceSqr.CompareTo(b.distanceSqr));
				int index = this.rng.RangeInt(list.Count * 3 / 4, list.Count);
				nodeIndex = list[index].nodeIndex;
			}
			else
			{
				nodeIndex = activeNodesForHullMask[this.rng.RangeInt(0, activeNodesForHullMask.Count)];
			}
			NodeGraphSpider nodeGraphSpider = new NodeGraphSpider(groundNodes, HullMask.Human);
			nodeGraphSpider.AddNodeForNextStep(nodeIndex);
			while (nodeGraphSpider.PerformStep())
			{
				List<NodeGraphSpider.StepInfo> collectedSteps = nodeGraphSpider.collectedSteps;
				if (usePod)
				{
					for (int k = collectedSteps.Count - 1; k >= 0; k--)
					{
						if (!SceneDirector.IsNodeSuitableForPod(groundNodes, collectedSteps[k].node))
						{
							collectedSteps.RemoveAt(k);
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
			int num = Math.Min(nodeGraphSpider.collectedSteps.Count, RoR2Application.maxPlayers);
			for (int l = 0; l < num; l++)
			{
				NodeGraph.NodeIndex node = collectedSteps2[l].node;
				Vector3 vector;
				groundNodes.GetNodePosition(node, out vector);
				NodeGraph.LinkIndex[] activeNodeLinks = groundNodes.GetActiveNodeLinks(node);
				Quaternion rotation;
				if (activeNodeLinks.Length != 0)
				{
					int num2 = this.rng.RangeInt(0, activeNodeLinks.Length);
					NodeGraph.LinkIndex linkIndex = activeNodeLinks[num2];
					Vector3 a2;
					groundNodes.GetNodePosition(groundNodes.GetLinkEndNode(linkIndex), out a2);
					rotation = Util.QuaternionSafeLookRotation(a2 - vector);
				}
				else
				{
					rotation = Quaternion.Euler(0f, this.rng.nextNormalizedFloat * 360f, 0f);
				}
				UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/SpawnPoint"), vector, rotation);
			}
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x000651B8 File Offset: 0x000633B8
		private void RemoveAllExistingSpawnPoints()
		{
			List<SpawnPoint> list = new List<SpawnPoint>(SpawnPoint.readOnlyInstancesList);
			for (int i = 0; i < list.Count; i++)
			{
				UnityEngine.Object.Destroy(list[i].gameObject);
			}
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x000651F4 File Offset: 0x000633F4
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

		// Token: 0x06001511 RID: 5393 RVA: 0x00065284 File Offset: 0x00063484
		private void PopulateScene()
		{
			ClassicStageInfo component = SceneInfo.instance.GetComponent<ClassicStageInfo>();
			this.PlaceTeleporter();
			if (SpawnPoint.readOnlyInstancesList.Count == 0 || Run.instance.stageClearCount > 0)
			{
				this.RemoveAllExistingSpawnPoints();
				this.PlacePlayerSpawnsViaNodegraph();
			}
			else
			{
				this.CullExistingSpawnPoints();
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
							GameObject gameObject = this.directorCore.TrySpawnObject(directorCard, placementRule, this.rng);
							if (gameObject)
							{
								PurchaseInteraction component2 = gameObject.GetComponent<PurchaseInteraction>();
								if (component2 && component2.costType == CostType.Money)
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
						GameObject gameObject2 = this.directorCore.TrySpawnObject(directorCard2.spawnCard, new DirectorPlacementRule
						{
							placementMode = DirectorPlacementRule.PlacementMode.Random
						}, this.rng);
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
					GameObject gameObject3 = DirectorCore.instance.TrySpawnObject(Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscLockbox"), new DirectorPlacementRule
					{
						placementMode = DirectorPlacementRule.PlacementMode.Random
					}, xoroshiro128Plus);
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

		// Token: 0x06001512 RID: 5394 RVA: 0x000655FC File Offset: 0x000637FC
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

		// Token: 0x04001856 RID: 6230
		private DirectorCore directorCore;

		// Token: 0x04001857 RID: 6231
		public SpawnCard teleporterSpawnCard;

		// Token: 0x04001858 RID: 6232
		public float expRewardCoefficient;

		// Token: 0x04001859 RID: 6233
		private int interactableCredit;

		// Token: 0x0400185A RID: 6234
		private int monsterCredit;

		// Token: 0x0400185B RID: 6235
		public GameObject teleporterInstance;

		// Token: 0x0400185C RID: 6236
		private Xoroshiro128Plus rng;

		// Token: 0x0400185D RID: 6237
		private static readonly WeightedSelection<DirectorCard> cardSelector = new WeightedSelection<DirectorCard>(8);

		// Token: 0x020003C9 RID: 969
		private struct NodeDistanceSqrPair
		{
			// Token: 0x0400185E RID: 6238
			public NodeGraph.NodeIndex nodeIndex;

			// Token: 0x0400185F RID: 6239
			public float distanceSqr;
		}
	}
}
