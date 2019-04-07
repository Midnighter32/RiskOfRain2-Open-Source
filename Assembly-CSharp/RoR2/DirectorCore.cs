using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002CF RID: 719
	public class DirectorCore : MonoBehaviour
	{
		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000E77 RID: 3703 RVA: 0x00047466 File Offset: 0x00045666
		// (set) Token: 0x06000E78 RID: 3704 RVA: 0x0004746D File Offset: 0x0004566D
		public static DirectorCore instance { get; private set; }

		// Token: 0x06000E79 RID: 3705 RVA: 0x00047475 File Offset: 0x00045675
		private void OnEnable()
		{
			if (!DirectorCore.instance)
			{
				DirectorCore.instance = this;
				return;
			}
			Debug.LogErrorFormat(this, "Duplicate instance of singleton class {0}. Only one should exist at a time.", new object[]
			{
				base.GetType().Name
			});
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x000474A9 File Offset: 0x000456A9
		private void OnDisable()
		{
			if (DirectorCore.instance == this)
			{
				DirectorCore.instance = null;
			}
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x000474C0 File Offset: 0x000456C0
		private void AddOccupiedNode(NodeGraph nodeGraph, NodeGraph.NodeIndex nodeIndex)
		{
			Array.Resize<DirectorCore.NodeReference>(ref this.occupiedNodes, this.occupiedNodes.Length + 1);
			this.occupiedNodes[this.occupiedNodes.Length - 1] = new DirectorCore.NodeReference
			{
				nodeGraph = nodeGraph,
				nodeIndex = nodeIndex
			};
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x00047510 File Offset: 0x00045710
		private bool CheckPositionFree(NodeGraph nodeGraph, NodeGraph.NodeIndex nodeIndex, SpawnCard spawnCard)
		{
			for (int i = 0; i < this.occupiedNodes.Length; i++)
			{
				if (this.occupiedNodes[i].nodeGraph == nodeGraph && this.occupiedNodes[i].nodeIndex == nodeIndex)
				{
					return false;
				}
			}
			float num = HullDef.Find(spawnCard.hullSize).radius * 0.7f;
			Vector3 vector;
			nodeGraph.GetNodePosition(nodeIndex, out vector);
			if (spawnCard.nodeGraphType == MapNodeGroup.GraphType.Ground)
			{
				vector += Vector3.up * (num + 0.25f);
			}
			return Physics.OverlapSphere(vector, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.fakeActor.mask).Length == 0;
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x000475E8 File Offset: 0x000457E8
		public GameObject TrySpawnObject(DirectorCard directorCard, DirectorPlacementRule placementRule, [NotNull] Xoroshiro128Plus rng)
		{
			return this.TrySpawnObject(directorCard.spawnCard, placementRule, rng);
		}

		// Token: 0x06000E7E RID: 3710 RVA: 0x000475F8 File Offset: 0x000457F8
		public GameObject TrySpawnObject(SpawnCard spawnCard, DirectorPlacementRule placementRule, [NotNull] Xoroshiro128Plus rng)
		{
			NodeGraph nodeGraph = SceneInfo.instance.GetNodeGraph(spawnCard.nodeGraphType);
			GameObject result = null;
			switch (placementRule.placementMode)
			{
			case DirectorPlacementRule.PlacementMode.Approximate:
			{
				List<NodeGraph.NodeIndex> list = nodeGraph.FindNodesInRangeWithFlagConditions(placementRule.targetPosition, placementRule.minDistance, placementRule.maxDistance, (HullMask)(1 << (int)spawnCard.hullSize), spawnCard.requiredFlags, spawnCard.forbiddenFlags, placementRule.preventOverhead);
				while (list.Count > 0)
				{
					int index = rng.RangeInt(0, list.Count);
					NodeGraph.NodeIndex nodeIndex = list[index];
					Vector3 position;
					nodeGraph.GetNodePosition(nodeIndex, out position);
					if (this.CheckPositionFree(nodeGraph, nodeIndex, spawnCard))
					{
						result = spawnCard.DoSpawn(position, Quaternion.identity);
						if (spawnCard.occupyPosition)
						{
							this.AddOccupiedNode(nodeGraph, nodeIndex);
							break;
						}
						break;
					}
					else
					{
						list.RemoveAt(index);
					}
				}
				break;
			}
			case DirectorPlacementRule.PlacementMode.ApproximateSimple:
			{
				NodeGraph.NodeIndex nodeIndex2 = nodeGraph.FindClosestNodeWithFlagConditions(placementRule.targetPosition, spawnCard.hullSize, spawnCard.requiredFlags, spawnCard.forbiddenFlags, placementRule.preventOverhead);
				Vector3 position2;
				if (nodeGraph.GetNodePosition(nodeIndex2, out position2))
				{
					if (this.CheckPositionFree(nodeGraph, nodeIndex2, spawnCard))
					{
						result = spawnCard.DoSpawn(position2, Quaternion.identity);
						if (spawnCard.occupyPosition)
						{
							this.AddOccupiedNode(nodeGraph, nodeIndex2);
						}
					}
					else
					{
						Debug.Log("Position not free.");
					}
				}
				else
				{
					Debug.Log("Could not find node.");
				}
				break;
			}
			case DirectorPlacementRule.PlacementMode.NearestNode:
			{
				NodeGraph.NodeIndex nodeIndex3 = nodeGraph.FindClosestNodeWithFlagConditions(placementRule.targetPosition, spawnCard.hullSize, spawnCard.requiredFlags, spawnCard.forbiddenFlags, placementRule.preventOverhead);
				Vector3 position3;
				if (nodeGraph.GetNodePosition(nodeIndex3, out position3))
				{
					result = spawnCard.DoSpawn(position3, Quaternion.identity);
					if (spawnCard.occupyPosition)
					{
						this.AddOccupiedNode(nodeGraph, nodeIndex3);
					}
				}
				break;
			}
			case DirectorPlacementRule.PlacementMode.Random:
			{
				List<NodeGraph.NodeIndex> activeNodesForHullMaskWithFlagConditions = nodeGraph.GetActiveNodesForHullMaskWithFlagConditions((HullMask)(1 << (int)spawnCard.hullSize), spawnCard.requiredFlags, spawnCard.forbiddenFlags);
				while (activeNodesForHullMaskWithFlagConditions.Count > 0)
				{
					int index2 = rng.RangeInt(0, activeNodesForHullMaskWithFlagConditions.Count);
					NodeGraph.NodeIndex nodeIndex4 = activeNodesForHullMaskWithFlagConditions[index2];
					Vector3 position4;
					if (nodeGraph.GetNodePosition(nodeIndex4, out position4) && this.CheckPositionFree(nodeGraph, nodeIndex4, spawnCard))
					{
						result = spawnCard.DoSpawn(position4, Quaternion.identity);
						if (spawnCard.occupyPosition)
						{
							this.AddOccupiedNode(nodeGraph, nodeIndex4);
							break;
						}
						break;
					}
					else
					{
						activeNodesForHullMaskWithFlagConditions.RemoveAt(index2);
					}
				}
				break;
			}
			}
			return result;
		}

		// Token: 0x06000E7F RID: 3711 RVA: 0x0004784C File Offset: 0x00045A4C
		public static void GetMonsterSpawnDistance(DirectorCore.MonsterSpawnDistance input, out float minimumDistance, out float maximumDistance)
		{
			minimumDistance = 0f;
			maximumDistance = 0f;
			switch (input)
			{
			case DirectorCore.MonsterSpawnDistance.Standard:
				minimumDistance = 25f;
				maximumDistance = 40f;
				return;
			case DirectorCore.MonsterSpawnDistance.Close:
				minimumDistance = 8f;
				maximumDistance = 20f;
				return;
			case DirectorCore.MonsterSpawnDistance.Far:
				minimumDistance = 70f;
				maximumDistance = 120f;
				return;
			default:
				return;
			}
		}

		// Token: 0x0400127E RID: 4734
		public const int maxTeamMemberCount = 40;

		// Token: 0x04001280 RID: 4736
		private DirectorCore.NodeReference[] occupiedNodes = Array.Empty<DirectorCore.NodeReference>();

		// Token: 0x020002D0 RID: 720
		private struct NodeReference
		{
			// Token: 0x04001281 RID: 4737
			public NodeGraph nodeGraph;

			// Token: 0x04001282 RID: 4738
			public NodeGraph.NodeIndex nodeIndex;
		}

		// Token: 0x020002D1 RID: 721
		public enum MonsterSpawnDistance
		{
			// Token: 0x04001284 RID: 4740
			Standard,
			// Token: 0x04001285 RID: 4741
			Close,
			// Token: 0x04001286 RID: 4742
			Far
		}
	}
}
