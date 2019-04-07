using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x0200052B RID: 1323
	[CreateAssetMenu]
	public class NodeGraph : ScriptableObject
	{
		// Token: 0x06001DA5 RID: 7589 RVA: 0x0008A89E File Offset: 0x00088A9E
		public void Clear()
		{
			this.nodes = Array.Empty<NodeGraph.Node>();
			this.links = Array.Empty<NodeGraph.Link>();
			this.gateNames = new List<string>
			{
				""
			};
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x0008A8CC File Offset: 0x00088ACC
		public void SetNodes(ReadOnlyCollection<MapNode> mapNodes, ReadOnlyCollection<SerializableBitArray> lineOfSightMasks)
		{
			this.Clear();
			Dictionary<MapNode, NodeGraph.NodeIndex> dictionary = new Dictionary<MapNode, NodeGraph.NodeIndex>();
			List<NodeGraph.Node> list = new List<NodeGraph.Node>();
			List<NodeGraph.Link> list2 = new List<NodeGraph.Link>();
			for (int i = 0; i < mapNodes.Count; i++)
			{
				MapNode key = mapNodes[i];
				dictionary[key] = new NodeGraph.NodeIndex
				{
					nodeIndex = i
				};
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

		// Token: 0x06001DA7 RID: 7591 RVA: 0x0008AB50 File Offset: 0x00088D50
		public Vector3 GetQuadraticCoordinates(float t, Vector3 startPos, Vector3 apexPos, Vector3 endPos)
		{
			return Mathf.Pow(1f - t, 2f) * startPos + 2f * t * (1f - t) * apexPos + Mathf.Pow(t, 2f) * endPos;
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x0008ABA8 File Offset: 0x00088DA8
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

		// Token: 0x06001DA9 RID: 7593 RVA: 0x0008AD24 File Offset: 0x00088F24
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

		// Token: 0x06001DAA RID: 7594 RVA: 0x0008AE78 File Offset: 0x00089078
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

		// Token: 0x06001DAB RID: 7595 RVA: 0x0008AF24 File Offset: 0x00089124
		public void DebugHighlightNodesWithNoLinks()
		{
			foreach (NodeGraph.Node node in this.nodes)
			{
				if (node.linkListIndex.size <= 0u)
				{
					Debug.DrawRay(node.position, Vector3.up * 100f, Color.cyan, 60f);
				}
			}
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x0008AF80 File Offset: 0x00089180
		public int GetNodeCount()
		{
			return this.nodes.Length;
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x0008AF8C File Offset: 0x0008918C
		public List<NodeGraph.NodeIndex> GetActiveNodesForHullMask(HullMask hullMask)
		{
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>(this.nodes.Length);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if ((this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					list.Add(new NodeGraph.NodeIndex
					{
						nodeIndex = i
					});
				}
			}
			return list;
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x0008B014 File Offset: 0x00089214
		public List<NodeGraph.NodeIndex> GetActiveNodesForHullMaskWithFlagConditions(HullMask hullMask, NodeFlags requiredFlags, NodeFlags forbiddenFlags)
		{
			List<NodeGraph.NodeIndex> list = new List<NodeGraph.NodeIndex>(this.nodes.Length);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				NodeFlags flags = this.nodes[i].flags;
				if ((flags & forbiddenFlags) == NodeFlags.None && (flags & requiredFlags) == requiredFlags && (this.nodes[i].forbiddenHulls & hullMask) == HullMask.None && (this.nodes[i].gateIndex == 0 || this.openGates[(int)this.nodes[i].gateIndex]))
				{
					list.Add(new NodeGraph.NodeIndex
					{
						nodeIndex = i
					});
				}
			}
			return list;
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x0008B0C4 File Offset: 0x000892C4
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
						list.Add(new NodeGraph.NodeIndex
						{
							nodeIndex = i
						});
					}
				}
			}
			return list;
		}

		// Token: 0x06001DB0 RID: 7600 RVA: 0x0008B180 File Offset: 0x00089380
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
						list.Add(new NodeGraph.NodeIndex
						{
							nodeIndex = i
						});
					}
				}
			}
			return list;
		}

		// Token: 0x06001DB1 RID: 7601 RVA: 0x0008B290 File Offset: 0x00089490
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

		// Token: 0x06001DB2 RID: 7602 RVA: 0x0008B2E4 File Offset: 0x000894E4
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

		// Token: 0x06001DB3 RID: 7603 RVA: 0x0008B324 File Offset: 0x00089524
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

		// Token: 0x06001DB4 RID: 7604 RVA: 0x0008B3AC File Offset: 0x000895AC
		public bool TestNodeLineOfSight(NodeGraph.NodeIndex nodeIndexA, NodeGraph.NodeIndex nodeIndexB)
		{
			return nodeIndexA != NodeGraph.NodeIndex.invalid && nodeIndexA.nodeIndex < this.nodes.Length && nodeIndexB != NodeGraph.NodeIndex.invalid && nodeIndexB.nodeIndex < this.nodes.Length && this.nodes[nodeIndexA.nodeIndex].lineOfSightMask[nodeIndexB.nodeIndex];
		}

		// Token: 0x06001DB5 RID: 7605 RVA: 0x0008B418 File Offset: 0x00089618
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

		// Token: 0x06001DB6 RID: 7606 RVA: 0x0008B4B4 File Offset: 0x000896B4
		public bool IsLinkSuitableForHull(NodeGraph.LinkIndex linkIndex, HullClassification hullClassification)
		{
			return linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length && (this.links[linkIndex.linkIndex].hullMask & 1 << (int)hullClassification) != 0 && (this.links[linkIndex.linkIndex].gateIndex == 0 || this.openGates[(int)this.links[linkIndex.linkIndex].gateIndex]);
		}

		// Token: 0x06001DB7 RID: 7607 RVA: 0x0008B538 File Offset: 0x00089738
		public bool IsLinkSuitableForHull(NodeGraph.LinkIndex linkIndex, HullMask hullMask)
		{
			return linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length && (this.links[linkIndex.linkIndex].hullMask & (int)hullMask) != 0 && (this.links[linkIndex.linkIndex].gateIndex == 0 || this.openGates[(int)this.links[linkIndex.linkIndex].gateIndex]);
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x0008B5B7 File Offset: 0x000897B7
		public NodeGraph.NodeIndex GetLinkStartNode(NodeGraph.LinkIndex linkIndex)
		{
			if (linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length)
			{
				return this.links[linkIndex.linkIndex].nodeIndexA;
			}
			return NodeGraph.NodeIndex.invalid;
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x0008B5F2 File Offset: 0x000897F2
		public NodeGraph.NodeIndex GetLinkEndNode(NodeGraph.LinkIndex linkIndex)
		{
			if (linkIndex != NodeGraph.LinkIndex.invalid && linkIndex.linkIndex < this.links.Length)
			{
				return this.links[linkIndex.linkIndex].nodeIndexB;
			}
			return NodeGraph.NodeIndex.invalid;
		}

		// Token: 0x06001DBA RID: 7610 RVA: 0x0008B630 File Offset: 0x00089830
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
						invalid.nodeIndex = i;
					}
				}
			}
			return invalid;
		}

		// Token: 0x06001DBB RID: 7611 RVA: 0x0008B6C0 File Offset: 0x000898C0
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
						invalid.nodeIndex = i;
					}
				}
			}
			return invalid;
		}

		// Token: 0x06001DBC RID: 7612 RVA: 0x0008B7BD File Offset: 0x000899BD
		private float HeuristicCostEstimate(Vector3 startPos, Vector3 endPos)
		{
			return Vector3.Distance(startPos, endPos);
		}

		// Token: 0x06001DBD RID: 7613 RVA: 0x00078729 File Offset: 0x00076929
		private static float DistanceXZ(Vector3 a, Vector3 b)
		{
			a.y = 0f;
			b.y = 0f;
			return Vector3.Distance(a, b);
		}

		// Token: 0x06001DBE RID: 7614 RVA: 0x0008B7C8 File Offset: 0x000899C8
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

		// Token: 0x06001DBF RID: 7615 RVA: 0x0008B804 File Offset: 0x00089A04
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
						invalid.nodeIndex = nodeIndex3;
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

		// Token: 0x06001DC0 RID: 7616 RVA: 0x0008BC68 File Offset: 0x00089E68
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

		// Token: 0x06001DC1 RID: 7617 RVA: 0x0008BCE0 File Offset: 0x00089EE0
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

		// Token: 0x06001DC2 RID: 7618 RVA: 0x0008BEA8 File Offset: 0x0008A0A8
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

		// Token: 0x06001DC3 RID: 7619 RVA: 0x0008BF10 File Offset: 0x0008A110
		public bool IsGateOpen(string gateName)
		{
			int num = this.gateNames.IndexOf(gateName);
			return num != -1 && this.openGates[num];
		}

		// Token: 0x06001DC4 RID: 7620 RVA: 0x0008BF38 File Offset: 0x0008A138
		public void SetGateState(string gateName, bool open)
		{
			int num = this.gateNames.IndexOf(gateName);
			if (num == -1)
			{
				return;
			}
			this.openGates[num] = open;
		}

		// Token: 0x04001FF6 RID: 8182
		[SerializeField]
		private NodeGraph.Node[] nodes = Array.Empty<NodeGraph.Node>();

		// Token: 0x04001FF7 RID: 8183
		[SerializeField]
		private NodeGraph.Link[] links = Array.Empty<NodeGraph.Link>();

		// Token: 0x04001FF8 RID: 8184
		[SerializeField]
		private List<string> gateNames = new List<string>
		{
			""
		};

		// Token: 0x04001FF9 RID: 8185
		private bool[] openGates = new bool[256];

		// Token: 0x04001FFA RID: 8186
		private const float overheadDotLimit = 0.70710677f;

		// Token: 0x0200052C RID: 1324
		[Serializable]
		public struct NodeIndex
		{
			// Token: 0x06001DC6 RID: 7622 RVA: 0x0008BFAF File Offset: 0x0008A1AF
			public static bool operator ==(NodeGraph.NodeIndex lhs, NodeGraph.NodeIndex rhs)
			{
				return lhs.nodeIndex == rhs.nodeIndex;
			}

			// Token: 0x06001DC7 RID: 7623 RVA: 0x0008BFBF File Offset: 0x0008A1BF
			public static bool operator !=(NodeGraph.NodeIndex lhs, NodeGraph.NodeIndex rhs)
			{
				return lhs.nodeIndex != rhs.nodeIndex;
			}

			// Token: 0x06001DC8 RID: 7624 RVA: 0x0008BFD2 File Offset: 0x0008A1D2
			public override bool Equals(object other)
			{
				return other is NodeGraph.NodeIndex && ((NodeGraph.NodeIndex)other).nodeIndex == this.nodeIndex;
			}

			// Token: 0x06001DC9 RID: 7625 RVA: 0x0008BFF1 File Offset: 0x0008A1F1
			public override int GetHashCode()
			{
				return this.nodeIndex;
			}

			// Token: 0x04001FFB RID: 8187
			public int nodeIndex;

			// Token: 0x04001FFC RID: 8188
			public static readonly NodeGraph.NodeIndex invalid = new NodeGraph.NodeIndex
			{
				nodeIndex = -1
			};
		}

		// Token: 0x0200052D RID: 1325
		[Serializable]
		public struct LinkIndex
		{
			// Token: 0x06001DCB RID: 7627 RVA: 0x0008C01F File Offset: 0x0008A21F
			public static bool operator ==(NodeGraph.LinkIndex lhs, NodeGraph.LinkIndex rhs)
			{
				return lhs.linkIndex == rhs.linkIndex;
			}

			// Token: 0x06001DCC RID: 7628 RVA: 0x0008C02F File Offset: 0x0008A22F
			public static bool operator !=(NodeGraph.LinkIndex lhs, NodeGraph.LinkIndex rhs)
			{
				return lhs.linkIndex != rhs.linkIndex;
			}

			// Token: 0x06001DCD RID: 7629 RVA: 0x0008C042 File Offset: 0x0008A242
			public override bool Equals(object other)
			{
				return other is NodeGraph.LinkIndex && ((NodeGraph.LinkIndex)other).linkIndex == this.linkIndex;
			}

			// Token: 0x06001DCE RID: 7630 RVA: 0x0008C061 File Offset: 0x0008A261
			public override int GetHashCode()
			{
				return this.linkIndex;
			}

			// Token: 0x04001FFD RID: 8189
			public int linkIndex;

			// Token: 0x04001FFE RID: 8190
			public static readonly NodeGraph.LinkIndex invalid = new NodeGraph.LinkIndex
			{
				linkIndex = -1
			};
		}

		// Token: 0x0200052E RID: 1326
		[Serializable]
		public struct LinkListIndex
		{
			// Token: 0x04001FFF RID: 8191
			public int index;

			// Token: 0x04002000 RID: 8192
			public uint size;
		}

		// Token: 0x0200052F RID: 1327
		[Serializable]
		public struct Node
		{
			// Token: 0x04002001 RID: 8193
			public Vector3 position;

			// Token: 0x04002002 RID: 8194
			public NodeGraph.LinkListIndex linkListIndex;

			// Token: 0x04002003 RID: 8195
			public HullMask forbiddenHulls;

			// Token: 0x04002004 RID: 8196
			public SerializableBitArray lineOfSightMask;

			// Token: 0x04002005 RID: 8197
			public byte gateIndex;

			// Token: 0x04002006 RID: 8198
			public NodeFlags flags;
		}

		// Token: 0x02000530 RID: 1328
		[Serializable]
		public struct Link
		{
			// Token: 0x04002007 RID: 8199
			public NodeGraph.NodeIndex nodeIndexA;

			// Token: 0x04002008 RID: 8200
			public NodeGraph.NodeIndex nodeIndexB;

			// Token: 0x04002009 RID: 8201
			public float distanceScore;

			// Token: 0x0400200A RID: 8202
			public float maxSlope;

			// Token: 0x0400200B RID: 8203
			public float minJumpHeight;

			// Token: 0x0400200C RID: 8204
			public int hullMask;

			// Token: 0x0400200D RID: 8205
			public int jumpHullMask;

			// Token: 0x0400200E RID: 8206
			public byte gateIndex;
		}

		// Token: 0x02000531 RID: 1329
		public class PathRequest
		{
			// Token: 0x0400200F RID: 8207
			public Path path;

			// Token: 0x04002010 RID: 8208
			public Vector3 startPos;

			// Token: 0x04002011 RID: 8209
			public Vector3 endPos;

			// Token: 0x04002012 RID: 8210
			public HullClassification hullClassification;

			// Token: 0x04002013 RID: 8211
			public float maxSlope;

			// Token: 0x04002014 RID: 8212
			public float maxJumpHeight;

			// Token: 0x04002015 RID: 8213
			public float maxSpeed;
		}
	}
}
