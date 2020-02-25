using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001E1 RID: 481
	public class DirectorCore : MonoBehaviour
	{
		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000A1F RID: 2591 RVA: 0x0002C2CC File Offset: 0x0002A4CC
		// (set) Token: 0x06000A20 RID: 2592 RVA: 0x0002C2D3 File Offset: 0x0002A4D3
		public static DirectorCore instance { get; private set; }

		// Token: 0x06000A21 RID: 2593 RVA: 0x0002C2DB File Offset: 0x0002A4DB
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

		// Token: 0x06000A22 RID: 2594 RVA: 0x0002C30F File Offset: 0x0002A50F
		private void OnDisable()
		{
			if (DirectorCore.instance == this)
			{
				DirectorCore.instance = null;
			}
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x0002C324 File Offset: 0x0002A524
		public void AddOccupiedNode(NodeGraph nodeGraph, NodeGraph.NodeIndex nodeIndex)
		{
			Array.Resize<DirectorCore.NodeReference>(ref this.occupiedNodes, this.occupiedNodes.Length + 1);
			this.occupiedNodes[this.occupiedNodes.Length - 1] = new DirectorCore.NodeReference(nodeGraph, nodeIndex);
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x0002C358 File Offset: 0x0002A558
		private bool CheckPositionFree(NodeGraph nodeGraph, NodeGraph.NodeIndex nodeIndex, SpawnCard spawnCard)
		{
			DirectorCore.NodeReference value = new DirectorCore.NodeReference(nodeGraph, nodeIndex);
			if (Array.IndexOf<DirectorCore.NodeReference>(this.occupiedNodes, value) != -1)
			{
				return false;
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

		// Token: 0x06000A25 RID: 2597 RVA: 0x0002C404 File Offset: 0x0002A604
		public GameObject TrySpawnObject([NotNull] DirectorSpawnRequest directorSpawnRequest)
		{
			SpawnCard spawnCard = directorSpawnRequest.spawnCard;
			DirectorPlacementRule placementRule = directorSpawnRequest.placementRule;
			Xoroshiro128Plus rng = directorSpawnRequest.rng;
			NodeGraph nodeGraph = SceneInfo.instance.GetNodeGraph(spawnCard.nodeGraphType);
			GameObject result = null;
			switch (placementRule.placementMode)
			{
			case DirectorPlacementRule.PlacementMode.Direct:
				result = spawnCard.DoSpawn(placementRule.spawnOnTarget ? placementRule.spawnOnTarget.position : directorSpawnRequest.placementRule.position, Quaternion.identity, directorSpawnRequest);
				break;
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
						result = spawnCard.DoSpawn(position, Quaternion.identity, directorSpawnRequest);
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
						result = spawnCard.DoSpawn(position2, Quaternion.identity, directorSpawnRequest);
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
					result = spawnCard.DoSpawn(position3, Quaternion.identity, directorSpawnRequest);
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
						result = spawnCard.DoSpawn(position4, Quaternion.identity, directorSpawnRequest);
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

		// Token: 0x06000A26 RID: 2598 RVA: 0x0002C6B8 File Offset: 0x0002A8B8
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

		// Token: 0x04000A74 RID: 2676
		public const int maxTeamMemberCount = 40;

		// Token: 0x04000A76 RID: 2678
		private DirectorCore.NodeReference[] occupiedNodes = Array.Empty<DirectorCore.NodeReference>();

		// Token: 0x020001E2 RID: 482
		private struct NodeReference : IEquatable<DirectorCore.NodeReference>
		{
			// Token: 0x06000A28 RID: 2600 RVA: 0x0002C725 File Offset: 0x0002A925
			public NodeReference(NodeGraph nodeGraph, NodeGraph.NodeIndex nodeIndex)
			{
				this.nodeGraph = nodeGraph;
				this.nodeIndex = nodeIndex;
			}

			// Token: 0x06000A29 RID: 2601 RVA: 0x0002C738 File Offset: 0x0002A938
			public bool Equals(DirectorCore.NodeReference other)
			{
				return object.Equals(this.nodeGraph, other.nodeGraph) && this.nodeIndex.Equals(other.nodeIndex);
			}

			// Token: 0x06000A2A RID: 2602 RVA: 0x0002C770 File Offset: 0x0002A970
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is DirectorCore.NodeReference)
				{
					DirectorCore.NodeReference other = (DirectorCore.NodeReference)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x06000A2B RID: 2603 RVA: 0x0002C79C File Offset: 0x0002A99C
			public override int GetHashCode()
			{
				return ((this.nodeGraph != null) ? this.nodeGraph.GetHashCode() : 0) * 397 ^ this.nodeIndex.GetHashCode();
			}

			// Token: 0x04000A77 RID: 2679
			public readonly NodeGraph nodeGraph;

			// Token: 0x04000A78 RID: 2680
			public readonly NodeGraph.NodeIndex nodeIndex;
		}

		// Token: 0x020001E3 RID: 483
		public enum MonsterSpawnDistance
		{
			// Token: 0x04000A7A RID: 2682
			Standard,
			// Token: 0x04000A7B RID: 2683
			Close,
			// Token: 0x04000A7C RID: 2684
			Far
		}
	}
}
