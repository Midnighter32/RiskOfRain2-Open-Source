using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x020004E5 RID: 1253
	[CreateAssetMenu]
	public class NodeGraph : ScriptableObject
	{
		// Token: 0x06001DD8 RID: 7640 RVA: 0x0007FBDA File Offset: 0x0007DDDA
		public void Clear()
		{
			this.nodes = Array.Empty<NodeGraph.Node>();
			this.links = Array.Empty<NodeGraph.Link>();
			this.gateNames = new List<string>
			{
				""
			};
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x0007FC08 File Offset: 0x0007DE08
		public void SetNodes(ReadOnlyCollection<MapNode> mapNodes, ReadOnlyCollection<SerializableBitArray> lineOfSightMasks)
		{
			this.Clear();
			Dictionary<MapNode, NodeGraph.NodeIndex> dictionary = new Dictionary<MapNode, NodeGraph.NodeIndex>();
			List<NodeGraph.Node> list = new List<NodeGraph.Node>();
			List<NodeGraph.Link> list2 = new List<NodeGraph.Link>();
			for (int i = 0; i < mapNodes.Count; i++)
			{
				MapNode key = mapNodes[i];
				dictionary[key] = new NodeGraph.NodeIndex(i);
			}
			for (int j = 0; j < mapNodes.Count; j++)
			{
				MapNode mapNode = mapNodes[j];
				NodeGraph.NodeIndex nodeIndexA = dictionary[mapNode];
				int count = list2.Count;
				for (int k = 0; k < mapNode.links.Count; k++)
				{
					MapNode.Link link = mapNode.links[k];
					if (!dictionary.ContainsKey(link.nodeB))
					{
						Debug.LogErrorFormat(link.nodeB, "[{0}] Node {1} was not registered.", new object[]
						{
							k,
							link.nodeB
						});
					}
					list2.Add(new NodeGraph.Link
					{
						nodeIndexA = nodeIndexA,
						nodeIndexB = dictionary[link.nodeB],
						distanceScore = link.distanceScore,
						minJumpHeight = link.minJumpHeight,
						hullMask = link.hullMask,
						jumpHullMask = link.jumpHullMask,
						gateIndex = this.RegisterGateName(link.gateName)
					});
				}
				HullMask hullMask = mapNode.forbiddenHulls;
				for (HullClassification hullClassification = HullClassification.Human; hullClassification < HullClassification.Count; hullClassification++)
				{
					bool flag = false;
					int num = 1 << (int)hullClassification;
					List<MapNode.Link> list3 = mapNode.links;
					for (int l = 0; l < list3.Count; l++)
					{
						if ((list3[l].hullMask & num) != 0)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						hullMask |= (HullMask)num;
					}
				}
				list.Add(new NodeGraph.Node
				{
					position = mapNode.transform.position,
					linkListIndex = new NodeGraph.LinkListIndex
					{
						index = count,
						size = (uint)mapNode.links.Count
					},
					forbiddenHulls = hullMask,
					flags = mapNode.flags,
					lineOfSightMask = new SerializableBitArray(lineOfSightMasks[j]),
					gateIndex = this.RegisterGateName(mapNode.gateName)
				});
			}
			this.nodes = list.ToArray();
			this.links = list2.ToArray();
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x0007FE80 File Offset: 0x0007E080
		public Vector3 GetQuadraticCoordinates(float t, Vector3 startPos, Vector3 apexPos, Vector3 endPos)
		{
			return Mathf.Pow(1f - t, 2f) * startPos + 2f * t * (1f - t) * apexPos + Mathf.Pow(t, 2f) * endPos;
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x0007FED8 File Offset: 0x0007E0D8
		public Mesh GenerateLinkDebugMesh(HullClassification hull)
		{
			Mesh result;
			using (WireMeshBuilder wireMeshBuilder = new WireMeshBuilder())
			{
				int num = 1 << (int)hull;
				foreach (NodeGraph.Link link in this.links)
				{
					if ((link.hullMask & num) != 0)
					{
						Vector3 position = this.nodes[link.nodeIndexA.nodeIndex].position;
						Vector3 position2 = this.nodes[link.nodeIndexB.nodeIndex].position;
						Vector3 vector = (position + position2) * 0.5f;
						bool flag = (link.jumpHullMask & num) != 0;
						Color color = flag ? Color.cyan : Color.green;
						if (flag)
						{
							Vector3 apexPos = vector;
							apexPos.y = position.y + link.minJumpHeight;
							int num2 = 8;
							Vector3 p = position;
							for (int j = 1; j <= num2; j++)
							{
								if (j > num2 / 2)
								{
									color.a = 0.1f;
								}
								Vector3 quadraticCoordinates = this.GetQuadraticCoordinates((float)j / (float)num2, position, apexPos, position2);
								wireMeshBuilder.AddLine(p, color, quadraticCoordinates, color);
								p = quadraticCoordinates;
							}
						}
						else
						{
							Color c = color;
							c.a = 0.1f;
							wireMeshBuilder.AddLine(position, color, position2, c);
						}
					}
				}
				result = wireMeshBuilder.GenerateMesh();
			}
			return result;
		}

		// Token: 0x06001DDC RID: 7644 RVA: 0x00080054 File Offset: 0x0007E254
		public void DebugDrawLinks(HullClassification hull)
		{
			int num = 1 << (int)hull;
			foreach (NodeGraph.Link link in this.links)
			{
				if ((link.hullMask & num) != 0)
				{
					Vector3 position = this.nodes[link.nodeIndexA.nodeIndex].position;
					Vector3 position2 = this.nodes[link.nodeIndexB.nodeIndex].position;
					Vector3 vector = (position + position2) * 0.5f;
					bool flag = (link.jumpHullMask & num) != 0;
					Color color = flag ? Color.cyan : Color.green;
					if (flag)
					{
						Vector3 apexPos = vector;
						apexPos.y = position.y + link.minJumpHeight;
						int num2 = 8;
						Vector3 start = position;
						for (int j = 1; j <= num2; j++)
						{
							if (j > num2 / 2)
							{
								color.a = 0.1f;
							}
							Vector3 quadraticCoordinates = this.GetQuadraticCoordinates((float)j / (float)num2, position, apexPos, position2);
							Debug.DrawLine(start, quadraticCoordinates, color, 10f);
							start = quadraticCoordinates;
						}
					}
					else
					{
						Debug.DrawLine(position, vector, color, 10f, false);
						Color color2 = color;
						color2.a = 0.1f;
						Debug.DrawLine(vector, position2, color2, 10f, false);
					}
				}
			}
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x000801A8 File Offset: 0x0007E3A8
		public void DebugDrawPath(Vector3 startPos, Vector3 endPos)
		{
			Path path = new Path(this);
			this.ComputePath(new NodeGraph.PathRequest
			{
				startPos = startPos,
				endPos = endPos,
				path = path,
				hullClassification = HullClassification.Human
			}).Wait();
			if (path.status == PathStatus.Valid)
			{
				for (int i = 1; i < path.waypointsCount; i++)
				{
					Debug.DrawLine(this.nodes[path[i - 1].nodeIndex.nodeIndex].position, this.nodes[path[i].nodeIndex.nodeIndex].position, Color.red, 10f);
				}
			}
		}

		// Token: 0x06001DDE RID: 7646 RVA: 0x00080254 File Offset: 0x0007E454
		public void DebugHighlightNodesWithNoLinks()
		{
			foreach (NodeGraph.Node node in this.nodes)
			{
				if (node.linkListIndex.size <= 0U)
				{
					Debug.DrawRay(node.position, Vector3.up * 100f, Color.cyan, 60f);
				}
			}
		}

		// Token: 0x06001DDF RID: 7647 RVA: 0x000802B0 File Offset: 0x0007E4B0
		public int GetNodeCount()
		{
			return this.nodes.Length;
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x000802BC File Offset: 0x0007E4BC
		public List<NodeGraph.NodeIndex> GetActiveNodesForHullMask(HullMask hullMask)
		{
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>(this.nodes.Length);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if ((this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					list.Add(new NodeGraph.NodeIndex(i));
				}
			}
			return list;
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x0008033C File Offset: 0x0007E53C
		public List<NodeGraph.NodeIndex> GetActiveNodesForHullMaskWithFlagConditions(HullMask hullMask, NodeFlags requiredFlags, NodeFlags forbiddenFlags)
		{
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>(this.nodes.Length);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				NodeFlags flags = this.nodes[i].flags;
				if ((flags & forbiddenFlags) == NodeFlags.None && (flags & requiredFlags) == requiredFlags && (this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					list.Add(new NodeGraph.NodeIndex(i));
				}
			}
			return list;
		}

		// Token: 0x06001DE2 RID: 7650 RVA: 0x000803DC File Offset: 0x0007E5DC
		public List<NodeGraph.NodeIndex> FindNodesInRange(Vector3 position, float minRange, float maxRange, HullMask hullMask)
		{
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>();
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if ((this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					float sqrMagnitude = (this.nodes[i].position - position).sqrMagnitude;
					if (sqrMagnitude >= num && sqrMagnitude <= num2)
					{
						list.Add(new NodeGraph.NodeIndex(i));
					}
				}
			}
			return list;
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x0008048C File Offset: 0x0007E68C
		public List<NodeGraph.NodeIndex> FindNodesInRangeWithFlagConditions(Vector3 position, float minRange, float maxRange, HullMask hullMask, NodeFlags requiredFlags, NodeFlags forbiddenFlags, bool preventOverhead)
		{
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>();
			for (int i = 0; i < this.nodes.Length; i++)
			{
				NodeFlags flags = this.nodes[i].flags;
				if ((flags & forbiddenFlags) == NodeFlags.None && (flags & requiredFlags) == requiredFlags && (this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					Vector3 a = this.nodes[i].position - position;
					float sqrMagnitude = a.sqrMagnitude;
					if (sqrMagnitude >= num && sqrMagnitude <= num2 && (!preventOverhead || Vector3.Dot(a / Mathf.Sqrt(sqrMagnitude), Vector3.up) <= 0.70710677f))
					{
						list.Add(new NodeGraph.NodeIndex(i));
					}
				}
			}
			return list;
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x00080590 File Offset: 0x0007E790
		public bool GetNodePosition(NodeGraph.NodeIndex nodeIndex, out Vector3 position)
		{
			if (nodeIndex != NodeGraph.NodeIndex.invalid && nodeIndex.nodeIndex < this.nodes.Length)
			{
				position = this.nodes[nodeIndex.nodeIndex].position;
				return true;
			}
			position = Vector3.zero;
			return false;
		}

		// Token: 0x06001DE5 RID: 7653 RVA: 0x000805E4 File Offset: 0x0007E7E4
		public bool GetNodeFlags(NodeGraph.NodeIndex nodeIndex, out NodeFlags flags)
		{
			if (nodeIndex != NodeGraph.NodeIndex.invalid && nodeIndex.nodeIndex < this.nodes.Length)
			{
				flags = this.nodes[nodeIndex.nodeIndex].flags;
				return true;
			}
			flags = NodeFlags.None;
			return false;
		}

		// Token: 0x06001DE6 RID: 7654 RVA: 0x00080624 File Offset: 0x0007E824
		public NodeGraph.LinkIndex[] GetActiveNodeLinks(NodeGraph.NodeIndex nodeIndex)
		{
			if (nodeIndex != NodeGraph.NodeIndex.invalid && nodeIndex.nodeIndex < this.nodes.Length)
			{
				NodeGraph.LinkListIndex linkListIndex = this.nodes[nodeIndex.nodeIndex].linkListIndex;
				NodeGraph.LinkIndex[] array = new NodeGraph.LinkIndex[linkListIndex.size];
				int index = linkListIndex.index;
				int num = 0;
				while ((long)num < (long)((ulong)linkListIndex.size))
				{
					array[num] = new NodeGraph.LinkIndex
					{
						linkIndex = index++
					};
					num++;
				}
				return array;
			}
			return null;
		}

		// Token: 0x06001DE7 RID: 7655 RVA: 0x000806AC File Offset: 0x0007E8AC
		public bool TestNodeLineOfSight(NodeGraph.NodeIndex nodeIndexA, NodeGraph.NodeIndex nodeIndexB)
		{
			return nodeIndexA != NodeGraph.NodeIndex.invalid && nodeIndexA.nodeIndex < this.nodes.Length && nodeIndexB != NodeGraph.NodeIndex.invalid && nodeIndexB.nodeIndex < this.nodes.Length && this.nodes[nodeIndexA.nodeIndex].lineOfSightMask[nodeIndexB.nodeIndex];
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x00080718 File Offset: 0x0007E918
		public bool GetPositionAlongLink(NodeGraph.LinkIndex linkIndex, float t, out Vector3 position)
		{
			if (linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length)
			{
				position = Vector3.LerpUnclamped(this.nodes[this.links[linkIndex.linkIndex].nodeIndexA.nodeIndex].position, this.nodes[this.links[linkIndex.linkIndex].nodeIndexB.nodeIndex].position, t);
				return true;
			}
			position = Vector3.zero;
			return false;
		}

		// Token: 0x06001DE9 RID: 7657 RVA: 0x000807B4 File Offset: 0x0007E9B4
		public bool IsLinkSuitableForHull(NodeGraph.LinkIndex linkIndex, HullClassification hullClassification)
		{
			return linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length && (this.links[linkIndex.linkIndex].hullMask & 1 << (int)hullClassification) != 0 && (this.links[linkIndex.linkIndex].gateIndex == 0 || this.openGates[(int)this.links[linkIndex.linkIndex].gateIndex]);
		}

		// Token: 0x06001DEA RID: 7658 RVA: 0x00080838 File Offset: 0x0007EA38
		public bool IsLinkSuitableForHull(NodeGraph.LinkIndex linkIndex, HullMask hullMask)
		{
			return linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length && (this.links[linkIndex.linkIndex].hullMask & (int)hullMask) != 0 && (this.links[linkIndex.linkIndex].gateIndex == 0 || this.openGates[(int)this.links[linkIndex.linkIndex].gateIndex]);
		}

		// Token: 0x06001DEB RID: 7659 RVA: 0x000808B7 File Offset: 0x0007EAB7
		public NodeGraph.NodeIndex GetLinkStartNode(NodeGraph.LinkIndex linkIndex)
		{
			if (linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length)
			{
				return this.links[linkIndex.linkIndex].nodeIndexA;
			}
			return NodeGraph.NodeIndex.invalid;
		}

		// Token: 0x06001DEC RID: 7660 RVA: 0x000808F2 File Offset: 0x0007EAF2
		public NodeGraph.NodeIndex GetLinkEndNode(NodeGraph.LinkIndex linkIndex)
		{
			if (linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length)
			{
				return this.links[linkIndex.linkIndex].nodeIndexB;
			}
			return NodeGraph.NodeIndex.invalid;
		}

		// Token: 0x06001DED RID: 7661 RVA: 0x00080930 File Offset: 0x0007EB30
		public NodeGraph.NodeIndex FindClosestNode(Vector3 position, HullClassification hullClassification)
		{
			float num = float.PositiveInfinity;
			NodeGraph.NodeIndex invalid = NodeGraph.NodeIndex.invalid;
			int num2 = 1 << (int)hullClassification;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				NodeGraph.Node node = this.nodes[i];
				if ((node.forbiddenHulls & (HullMask)num2) == HullMask.None && (node.gateIndex == 0 || this.openGates[(int)node.gateIndex]))
				{
					float sqrMagnitude = (node.position - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						invalid = new NodeGraph.NodeIndex(i);
					}
				}
			}
			return invalid;
		}

		// Token: 0x06001DEE RID: 7662 RVA: 0x000809C0 File Offset: 0x0007EBC0
		public NodeGraph.NodeIndex FindClosestNodeWithFlagConditions(Vector3 position, HullClassification hullClassification, NodeFlags requiredFlags, NodeFlags forbiddenFlags, bool preventOverhead)
		{
			float num = float.PositiveInfinity;
			NodeGraph.NodeIndex invalid = NodeGraph.NodeIndex.invalid;
			int num2 = 1 << (int)hullClassification;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				NodeFlags flags = this.nodes[i].flags;
				if ((flags & forbiddenFlags) == NodeFlags.None && (flags & requiredFlags) == requiredFlags && (this.nodes[i].forbiddenHulls & (HullMask)num2) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					Vector3 a = this.nodes[i].position - position;
					float sqrMagnitude = a.sqrMagnitude;
					if (sqrMagnitude < num && (!preventOverhead || Vector3.Dot(a / Mathf.Sqrt(sqrMagnitude), Vector3.up) <= 0.70710677f))
					{
						num = sqrMagnitude;
						invalid = new NodeGraph.NodeIndex(i);
					}
				}
			}
			return invalid;
		}

		// Token: 0x06001DEF RID: 7663 RVA: 0x00080ABD File Offset: 0x0007ECBD
		private float HeuristicCostEstimate(Vector3 startPos, Vector3 endPos)
		{
			return Vector3.Distance(startPos, endPos);
		}

		// Token: 0x06001DF0 RID: 7664 RVA: 0x00068305 File Offset: 0x00066505
		private static float DistanceXZ(Vector3 a, Vector3 b)
		{
			a.y = 0f;
			b.y = 0f;
			return Vector3.Distance(a, b);
		}

		// Token: 0x06001DF1 RID: 7665 RVA: 0x00080AC8 File Offset: 0x0007ECC8
		private static void ArrayRemoveNodeIndex(NodeGraph.NodeIndex[] array, NodeGraph.NodeIndex value, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (array[i] == value)
				{
					array[i] = array[count - 1];
					return;
				}
			}
		}

		// Token: 0x06001DF2 RID: 7666 RVA: 0x00080B04 File Offset: 0x0007ED04
		public PathTask ComputePath(NodeGraph.PathRequest pathRequest)
		{
			PathTask pathTask = new PathTask(pathRequest.path);
			pathTask.status = PathTask.TaskStatus.Running;
			NodeGraph.NodeIndex nodeIndex = this.FindClosestNode(pathRequest.startPos, pathRequest.hullClassification);
			NodeGraph.NodeIndex nodeIndex2 = this.FindClosestNode(pathRequest.endPos, pathRequest.hullClassification);
			if (nodeIndex.nodeIndex == NodeGraph.NodeIndex.invalid.nodeIndex || nodeIndex2.nodeIndex == NodeGraph.NodeIndex.invalid.nodeIndex)
			{
				pathRequest.path.Clear();
				pathTask.status = PathTask.TaskStatus.Complete;
				return pathTask;
			}
			int num = 1 << (int)pathRequest.hullClassification;
			bool[] array = new bool[this.nodes.Length];
			bool[] array2 = new bool[this.nodes.Length];
			array2[nodeIndex.nodeIndex] = true;
			int i = 1;
			NodeGraph.NodeIndex[] array3 = new NodeGraph.NodeIndex[this.nodes.Length];
			array3[0] = nodeIndex;
			NodeGraph.LinkIndex[] array4 = new NodeGraph.LinkIndex[this.nodes.Length];
			for (int j = 0; j < array4.Length; j++)
			{
				array4[j] = NodeGraph.LinkIndex.invalid;
			}
			float[] array5 = new float[this.nodes.Length];
			for (int k = 0; k < array5.Length; k++)
			{
				array5[k] = float.PositiveInfinity;
			}
			array5[nodeIndex.nodeIndex] = 0f;
			float[] array6 = new float[this.nodes.Length];
			for (int l = 0; l < array6.Length; l++)
			{
				array6[l] = float.PositiveInfinity;
			}
			array6[nodeIndex.nodeIndex] = this.HeuristicCostEstimate(pathRequest.startPos, pathRequest.endPos);
			while (i > 0)
			{
				NodeGraph.NodeIndex invalid = NodeGraph.NodeIndex.invalid;
				float num2 = float.PositiveInfinity;
				for (int m = 0; m < i; m++)
				{
					int nodeIndex3 = array3[m].nodeIndex;
					if (array6[nodeIndex3] <= num2)
					{
						num2 = array6[nodeIndex3];
						invalid = new NodeGraph.NodeIndex(nodeIndex3);
					}
				}
				if (invalid.nodeIndex == nodeIndex2.nodeIndex)
				{
					this.ReconstructPath(pathRequest.path, array4, array4[invalid.nodeIndex], pathRequest);
					pathTask.status = PathTask.TaskStatus.Complete;
					return pathTask;
				}
				array2[invalid.nodeIndex] = false;
				NodeGraph.ArrayRemoveNodeIndex(array3, invalid, i);
				i--;
				array[invalid.nodeIndex] = true;
				NodeGraph.LinkListIndex linkListIndex = this.nodes[invalid.nodeIndex].linkListIndex;
				NodeGraph.LinkIndex linkIndex = new NodeGraph.LinkIndex
				{
					linkIndex = linkListIndex.index
				};
				NodeGraph.LinkIndex linkIndex2 = new NodeGraph.LinkIndex
				{
					linkIndex = linkListIndex.index + (int)linkListIndex.size
				};
				while (linkIndex.linkIndex < linkIndex2.linkIndex)
				{
					NodeGraph.Link link = this.links[linkIndex.linkIndex];
					NodeGraph.NodeIndex nodeIndexB = link.nodeIndexB;
					if (!array[nodeIndexB.nodeIndex])
					{
						if ((num & link.jumpHullMask) != 0 && this.links[linkIndex.linkIndex].minJumpHeight > 0f)
						{
							Vector3 position = this.nodes[link.nodeIndexA.nodeIndex].position;
							Vector3 position2 = this.nodes[link.nodeIndexB.nodeIndex].position;
							if (Trajectory.CalculateApex(Trajectory.CalculateInitialYSpeed(Trajectory.CalculateGroundTravelTime(pathRequest.maxSpeed, NodeGraph.DistanceXZ(position, position2)), position2.y - position.y)) > pathRequest.maxJumpHeight)
							{
								goto IL_41A;
							}
						}
						if ((link.hullMask & num) != 0 && (link.gateIndex == 0 || this.openGates[(int)link.gateIndex]))
						{
							float num3 = array5[invalid.nodeIndex] + link.distanceScore;
							if (!array2[nodeIndexB.nodeIndex])
							{
								array2[nodeIndexB.nodeIndex] = true;
								array3[i] = nodeIndexB;
								i++;
							}
							else if (num3 >= array5[nodeIndexB.nodeIndex])
							{
								goto IL_41A;
							}
							array4[nodeIndexB.nodeIndex] = linkIndex;
							array5[nodeIndexB.nodeIndex] = num3;
							array6[nodeIndexB.nodeIndex] = array5[nodeIndexB.nodeIndex] + this.HeuristicCostEstimate(this.nodes[nodeIndexB.nodeIndex].position, this.nodes[nodeIndex2.nodeIndex].position);
						}
					}
					IL_41A:
					linkIndex.linkIndex++;
				}
			}
			pathRequest.path.Clear();
			pathTask.status = PathTask.TaskStatus.Complete;
			return pathTask;
		}

		// Token: 0x06001DF3 RID: 7667 RVA: 0x00080F68 File Offset: 0x0007F168
		private NodeGraph.LinkIndex Resolve(NodeGraph.LinkIndex[] cameFrom, NodeGraph.LinkIndex current)
		{
			if (current.linkIndex < 0 || current.linkIndex > this.links.Length)
			{
				Debug.LogFormat("Link {0} is out of range [0,{1})", new object[]
				{
					current.linkIndex,
					this.links.Length
				});
			}
			NodeGraph.NodeIndex nodeIndexA = this.links[current.linkIndex].nodeIndexA;
			return cameFrom[nodeIndexA.nodeIndex];
		}

		// Token: 0x06001DF4 RID: 7668 RVA: 0x00080FE0 File Offset: 0x0007F1E0
		private void ReconstructPath(Path path, NodeGraph.LinkIndex[] cameFrom, NodeGraph.LinkIndex current, NodeGraph.PathRequest pathRequest)
		{
			int num = 1 << (int)pathRequest.hullClassification;
			path.Clear();
			if (current != NodeGraph.LinkIndex.invalid)
			{
				path.PushWaypointToFront(this.links[current.linkIndex].nodeIndexB, 0f);
			}
			while (current != NodeGraph.LinkIndex.invalid)
			{
				NodeGraph.NodeIndex nodeIndexB = this.links[current.linkIndex].nodeIndexB;
				float minJumpHeight = 0f;
				if ((num & this.links[current.linkIndex].jumpHullMask) != 0 && this.links[current.linkIndex].minJumpHeight > 0f)
				{
					Vector3 position = this.nodes[this.links[current.linkIndex].nodeIndexA.nodeIndex].position;
					Vector3 position2 = this.nodes[this.links[current.linkIndex].nodeIndexB.nodeIndex].position;
					minJumpHeight = Trajectory.CalculateApex(Trajectory.CalculateInitialYSpeed(Trajectory.CalculateGroundTravelTime(pathRequest.maxSpeed, NodeGraph.DistanceXZ(position, position2)), position2.y - position.y));
				}
				path.PushWaypointToFront(nodeIndexB, minJumpHeight);
				if (cameFrom[this.links[current.linkIndex].nodeIndexA.nodeIndex] == NodeGraph.LinkIndex.invalid)
				{
					path.PushWaypointToFront(this.links[current.linkIndex].nodeIndexA, 0f);
				}
				current = cameFrom[this.links[current.linkIndex].nodeIndexA.nodeIndex];
			}
			path.status = PathStatus.Valid;
		}

		// Token: 0x06001DF5 RID: 7669 RVA: 0x000811A8 File Offset: 0x0007F3A8
		private byte RegisterGateName(string gateName)
		{
			if (string.IsNullOrEmpty(gateName))
			{
				return 0;
			}
			int num = this.gateNames.IndexOf(gateName);
			if (num == -1)
			{
				num = this.gateNames.Count;
				if (num >= 256)
				{
					Debug.LogErrorFormat(this, "Nodegraph cannot have more than 255 gate names. Nodegraph={0} gateName={1}", new object[]
					{
						this,
						gateName
					});
					num = 0;
				}
				else
				{
					this.gateNames.Add(gateName);
				}
			}
			return (byte)num;
		}

		// Token: 0x06001DF6 RID: 7670 RVA: 0x00081210 File Offset: 0x0007F410
		public bool IsGateOpen(string gateName)
		{
			int num = this.gateNames.IndexOf(gateName);
			return num != -1 && this.openGates[num];
		}

		// Token: 0x06001DF7 RID: 7671 RVA: 0x00081238 File Offset: 0x0007F438
		public void SetGateState(string gateName, bool open)
		{
			int num = this.gateNames.IndexOf(gateName);
			if (num == -1)
			{
				return;
			}
			this.openGates[num] = open;
		}

		// Token: 0x04001B0F RID: 6927
		[SerializeField]
		private NodeGraph.Node[] nodes = Array.Empty<NodeGraph.Node>();

		// Token: 0x04001B10 RID: 6928
		[SerializeField]
		private NodeGraph.Link[] links = Array.Empty<NodeGraph.Link>();

		// Token: 0x04001B11 RID: 6929
		[SerializeField]
		private List<string> gateNames = new List<string>
		{
			""
		};

		// Token: 0x04001B12 RID: 6930
		private bool[] openGates = new bool[256];

		// Token: 0x04001B13 RID: 6931
		private const float overheadDotLimit = 0.70710677f;

		// Token: 0x020004E6 RID: 1254
		[Serializable]
		public struct NodeIndex : IEquatable<NodeGraph.NodeIndex>
		{
			// Token: 0x06001DF9 RID: 7673 RVA: 0x000812AF File Offset: 0x0007F4AF
			public NodeIndex(int nodeIndex)
			{
				this.nodeIndex = nodeIndex;
			}

			// Token: 0x06001DFA RID: 7674 RVA: 0x000812B8 File Offset: 0x0007F4B8
			public static bool operator ==(NodeGraph.NodeIndex lhs, NodeGraph.NodeIndex rhs)
			{
				return lhs.nodeIndex == rhs.nodeIndex;
			}

			// Token: 0x06001DFB RID: 7675 RVA: 0x000812C8 File Offset: 0x0007F4C8
			public static bool operator !=(NodeGraph.NodeIndex lhs, NodeGraph.NodeIndex rhs)
			{
				return lhs.nodeIndex != rhs.nodeIndex;
			}

			// Token: 0x06001DFC RID: 7676 RVA: 0x000812DB File Offset: 0x0007F4DB
			public override bool Equals(object other)
			{
				return other is NodeGraph.NodeIndex && ((NodeGraph.NodeIndex)other).nodeIndex == this.nodeIndex;
			}

			// Token: 0x06001DFD RID: 7677 RVA: 0x000812FA File Offset: 0x0007F4FA
			public override int GetHashCode()
			{
				return this.nodeIndex;
			}

			// Token: 0x06001DFE RID: 7678 RVA: 0x000812B8 File Offset: 0x0007F4B8
			public bool Equals(NodeGraph.NodeIndex other)
			{
				return this.nodeIndex == other.nodeIndex;
			}

			// Token: 0x04001B14 RID: 6932
			public int nodeIndex;

			// Token: 0x04001B15 RID: 6933
			public static readonly NodeGraph.NodeIndex invalid = new NodeGraph.NodeIndex(-1);
		}

		// Token: 0x020004E7 RID: 1255
		[Serializable]
		public struct LinkIndex
		{
			// Token: 0x06001E00 RID: 7680 RVA: 0x0008130F File Offset: 0x0007F50F
			public static bool operator ==(NodeGraph.LinkIndex lhs, NodeGraph.LinkIndex rhs)
			{
				return lhs.linkIndex == rhs.linkIndex;
			}

			// Token: 0x06001E01 RID: 7681 RVA: 0x0008131F File Offset: 0x0007F51F
			public static bool operator !=(NodeGraph.LinkIndex lhs, NodeGraph.LinkIndex rhs)
			{
				return lhs.linkIndex != rhs.linkIndex;
			}

			// Token: 0x06001E02 RID: 7682 RVA: 0x00081332 File Offset: 0x0007F532
			public override bool Equals(object other)
			{
				return other is NodeGraph.LinkIndex && ((NodeGraph.LinkIndex)other).linkIndex == this.linkIndex;
			}

			// Token: 0x06001E03 RID: 7683 RVA: 0x00081351 File Offset: 0x0007F551
			public override int GetHashCode()
			{
				return this.linkIndex;
			}

			// Token: 0x04001B16 RID: 6934
			public int linkIndex;

			// Token: 0x04001B17 RID: 6935
			public static readonly NodeGraph.LinkIndex invalid = new NodeGraph.LinkIndex
			{
				linkIndex = -1
			};
		}

		// Token: 0x020004E8 RID: 1256
		[Serializable]
		public struct LinkListIndex
		{
			// Token: 0x04001B18 RID: 6936
			public int index;

			// Token: 0x04001B19 RID: 6937
			public uint size;
		}

		// Token: 0x020004E9 RID: 1257
		[Serializable]
		public struct Node
		{
			// Token: 0x04001B1A RID: 6938
			public Vector3 position;

			// Token: 0x04001B1B RID: 6939
			public NodeGraph.LinkListIndex linkListIndex;

			// Token: 0x04001B1C RID: 6940
			public HullMask forbiddenHulls;

			// Token: 0x04001B1D RID: 6941
			public SerializableBitArray lineOfSightMask;

			// Token: 0x04001B1E RID: 6942
			public byte gateIndex;

			// Token: 0x04001B1F RID: 6943
			public NodeFlags flags;
		}

		// Token: 0x020004EA RID: 1258
		[Serializable]
		public struct Link
		{
			// Token: 0x04001B20 RID: 6944
			public NodeGraph.NodeIndex nodeIndexA;

			// Token: 0x04001B21 RID: 6945
			public NodeGraph.NodeIndex nodeIndexB;

			// Token: 0x04001B22 RID: 6946
			public float distanceScore;

			// Token: 0x04001B23 RID: 6947
			public float maxSlope;

			// Token: 0x04001B24 RID: 6948
			public float minJumpHeight;

			// Token: 0x04001B25 RID: 6949
			public int hullMask;

			// Token: 0x04001B26 RID: 6950
			public int jumpHullMask;

			// Token: 0x04001B27 RID: 6951
			public byte gateIndex;
		}

		// Token: 0x020004EB RID: 1259
		public class PathRequest
		{
			// Token: 0x04001B28 RID: 6952
			public Path path;

			// Token: 0x04001B29 RID: 6953
			public Vector3 startPos;

			// Token: 0x04001B2A RID: 6954
			public Vector3 endPos;

			// Token: 0x04001B2B RID: 6955
			public HullClassification hullClassification;

			// Token: 0x04001B2C RID: 6956
			public float maxSlope;

			// Token: 0x04001B2D RID: 6957
			public float maxJumpHeight;

			// Token: 0x04001B2E RID: 6958
			public float maxSpeed;
		}
	}
}
