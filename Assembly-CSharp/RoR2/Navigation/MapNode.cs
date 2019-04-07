using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x02000521 RID: 1313
	[ExecuteInEditMode]
	public class MapNode : MonoBehaviour
	{
		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06001D7E RID: 7550 RVA: 0x00089731 File Offset: 0x00087931
		public static ReadOnlyCollection<MapNode> instances
		{
			get
			{
				return MapNode.instancesReadOnly;
			}
		}

		// Token: 0x06001D7F RID: 7551 RVA: 0x00089738 File Offset: 0x00087938
		public void OnEnable()
		{
			MapNode._instances.Add(this);
		}

		// Token: 0x06001D80 RID: 7552 RVA: 0x00089745 File Offset: 0x00087945
		public void OnDisable()
		{
			MapNode._instances.Remove(this);
		}

		// Token: 0x06001D81 RID: 7553 RVA: 0x00089754 File Offset: 0x00087954
		private void AddLink(MapNode nodeB, float distanceScore, float minJumpHeight, HullClassification hullClassification)
		{
			int num = this.links.FindIndex((MapNode.Link item) => item.nodeB == nodeB);
			if (num == -1)
			{
				this.links.Add(new MapNode.Link
				{
					nodeB = nodeB
				});
				num = this.links.Count - 1;
			}
			MapNode.Link link = this.links[num];
			link.distanceScore = Mathf.Max(link.distanceScore, distanceScore);
			link.minJumpHeight = Mathf.Max(link.minJumpHeight, minJumpHeight);
			link.hullMask |= 1 << (int)hullClassification;
			if (minJumpHeight > 0f)
			{
				link.jumpHullMask |= 1 << (int)hullClassification;
			}
			if (string.IsNullOrEmpty(link.gateName))
			{
				link.gateName = nodeB.gateName;
			}
			this.links[num] = link;
		}

		// Token: 0x06001D82 RID: 7554 RVA: 0x00089844 File Offset: 0x00087A44
		private void BuildGroundLinks(ReadOnlyCollection<MapNode> nodes, MapNode.MoveProbe moveProbe)
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < nodes.Count; i++)
			{
				MapNode mapNode = nodes[i];
				if (!(mapNode == this))
				{
					Vector3 position2 = mapNode.transform.position;
					float num = MapNode.maxConnectionDistance;
					float num2 = num * num;
					float sqrMagnitude = (position2 - position).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						float distanceScore = Mathf.Sqrt(sqrMagnitude);
						for (int j = 0; j < 3; j++)
						{
							moveProbe.SetHull((HullClassification)j);
							if ((this.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None && (mapNode.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None)
							{
								Vector3 b = Vector3.up * (moveProbe.testCharacterController.height * 0.5f);
								Vector3 b2 = Vector3.up * 0.01f;
								Vector3 a = moveProbe.GetGroundPosition(position) + b2;
								Vector3 a2 = moveProbe.GetGroundPosition(position2) + b2;
								Vector3 vector = a + b;
								Vector3 vector2 = a2 + b;
								if (moveProbe.CapsuleOverlapTest(vector) && moveProbe.CapsuleOverlapTest(vector2))
								{
									bool flag = moveProbe.GroundTest(vector, vector2, 6f);
									float num3 = (!flag) ? moveProbe.JumpTest(vector, vector2, 7.5f) : 0f;
									if (flag || (num3 > 0f && num3 < 10f))
									{
										this.AddLink(mapNode, distanceScore, num3, (HullClassification)j);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x000899C4 File Offset: 0x00087BC4
		private void BuildAirLinks(ReadOnlyCollection<MapNode> nodes, MapNode.MoveProbe moveProbe)
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < nodes.Count; i++)
			{
				MapNode mapNode = nodes[i];
				if (!(mapNode == this))
				{
					Vector3 position2 = mapNode.transform.position;
					float num = MapNode.maxConnectionDistance * 2f;
					float num2 = num * num;
					float sqrMagnitude = (position2 - position).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						float distanceScore = Mathf.Sqrt(sqrMagnitude);
						for (int j = 0; j < 3; j++)
						{
							if ((this.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None && (mapNode.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None)
							{
								moveProbe.SetHull((HullClassification)j);
								Vector3 vector = position;
								Vector3 vector2 = position2;
								if (moveProbe.CapsuleOverlapTest(vector) && moveProbe.CapsuleOverlapTest(vector2) && moveProbe.FlyTest(vector, vector2, 6f))
								{
									this.AddLink(mapNode, distanceScore, 0f, (HullClassification)j);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x00089AB8 File Offset: 0x00087CB8
		private void BuildRailLinks(ReadOnlyCollection<MapNode> nodes, MapNode.MoveProbe moveProbe)
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < nodes.Count; i++)
			{
				MapNode mapNode = nodes[i];
				if (!(mapNode == this))
				{
					Vector3 position2 = mapNode.transform.position;
					float num = MapNode.maxConnectionDistance * 2f;
					float num2 = num * num;
					float sqrMagnitude = (position2 - position).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						float distanceScore = Mathf.Sqrt(sqrMagnitude);
						for (int j = 0; j < 3; j++)
						{
							HullDef hullDef = HullDef.Find((HullClassification)j);
							if ((this.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None && (mapNode.forbiddenHulls & (HullMask)(1 << j)) == HullMask.None)
							{
								moveProbe.SetHull((HullClassification)j);
								Vector3 vector = position;
								Vector3 vector2 = position2;
								if (Vector3.Angle(Vector3.up, vector2 - vector) > 50f)
								{
									vector.y += hullDef.height;
									vector2.y += hullDef.height;
									if (moveProbe.CapsuleOverlapTest(vector) && moveProbe.CapsuleOverlapTest(vector2) && moveProbe.FlyTest(vector, vector2, 6f))
									{
										this.AddLink(mapNode, distanceScore, 0f, (HullClassification)j);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x00089C04 File Offset: 0x00087E04
		public void BuildLinks(ReadOnlyCollection<MapNode> nodes, MapNodeGroup.GraphType graphType)
		{
			this.links.Clear();
			Vector3 position = base.transform.position;
			MapNode.MoveProbe moveProbe = new MapNode.MoveProbe();
			moveProbe.Init();
			switch (graphType)
			{
			case MapNodeGroup.GraphType.Ground:
				this.BuildGroundLinks(nodes, moveProbe);
				break;
			case MapNodeGroup.GraphType.Air:
				this.BuildAirLinks(nodes, moveProbe);
				break;
			case MapNodeGroup.GraphType.Rail:
				this.BuildRailLinks(nodes, moveProbe);
				break;
			}
			foreach (MapNodeLink mapNodeLink in base.GetComponents<MapNodeLink>())
			{
				if (mapNodeLink.other)
				{
					MapNode.Link link = new MapNode.Link
					{
						nodeB = mapNodeLink.other,
						distanceScore = Vector3.Distance(position, mapNodeLink.other.transform.position),
						minJumpHeight = mapNodeLink.minJumpHeight,
						gateName = mapNodeLink.gateName,
						hullMask = -1
					};
					bool flag = false;
					for (int j = 0; j < this.links.Count; j++)
					{
						if (this.links[j].nodeB == mapNodeLink.other)
						{
							this.links[j] = link;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.links.Add(link);
					}
				}
			}
			moveProbe.Destroy();
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x00089D5C File Offset: 0x00087F5C
		public bool TestLineOfSight(MapNode other)
		{
			return !Physics.Linecast(base.transform.position + Vector3.up, other.transform.position + Vector3.up, LayerIndex.world.mask);
		}

		// Token: 0x06001D87 RID: 7559 RVA: 0x00089DB0 File Offset: 0x00087FB0
		public bool TestNoCeiling()
		{
			return !Physics.Raycast(new Ray(base.transform.position, Vector3.up), float.PositiveInfinity, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x00089DF4 File Offset: 0x00087FF4
		public bool TestTeleporterOK()
		{
			float d = 15f;
			int num = 20;
			float num2 = 7f;
			float num3 = 3f;
			float num4 = 360f / (float)num;
			for (int i = 0; i < num; i++)
			{
				Vector3 b = Quaternion.AngleAxis(num4 * (float)i, Vector3.up) * (Vector3.forward * d);
				Vector3 origin = base.transform.position + b + Vector3.up * num2;
				RaycastHit raycastHit;
				if (!Physics.Raycast(new Ray(origin, Vector3.down), out raycastHit, num3 + num2, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					return false;
				}
			}
			Debug.DrawRay(base.transform.position, base.transform.up * 20f, Color.green, 15f);
			return true;
		}

		// Token: 0x04001FCB RID: 8139
		private static List<MapNode> _instances = new List<MapNode>();

		// Token: 0x04001FCC RID: 8140
		private static ReadOnlyCollection<MapNode> instancesReadOnly = MapNode._instances.AsReadOnly();

		// Token: 0x04001FCD RID: 8141
		public static readonly float maxConnectionDistance = 15f;

		// Token: 0x04001FCE RID: 8142
		public List<MapNode.Link> links = new List<MapNode.Link>();

		// Token: 0x04001FCF RID: 8143
		public HullMask forbiddenHulls;

		// Token: 0x04001FD0 RID: 8144
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags flags;

		// Token: 0x04001FD1 RID: 8145
		[Tooltip("The name of the nodegraph gate associated with this node. If the named gate is closed this node will be treated as though it does not exist.")]
		public string gateName = "";

		// Token: 0x02000522 RID: 1314
		public struct Link
		{
			// Token: 0x04001FD2 RID: 8146
			public MapNode nodeB;

			// Token: 0x04001FD3 RID: 8147
			public float distanceScore;

			// Token: 0x04001FD4 RID: 8148
			public float minJumpHeight;

			// Token: 0x04001FD5 RID: 8149
			public int hullMask;

			// Token: 0x04001FD6 RID: 8150
			public int jumpHullMask;

			// Token: 0x04001FD7 RID: 8151
			public string gateName;
		}

		// Token: 0x02000523 RID: 1315
		private class MoveProbe
		{
			// Token: 0x06001D8B RID: 7563 RVA: 0x00089F18 File Offset: 0x00088118
			public void Init()
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "NodeGraphProbe";
				Transform transform = gameObject.transform;
				this.testCharacterController = gameObject.AddComponent<CharacterController>();
				this.testCharacterController.stepOffset = 0.5f;
				this.testCharacterController.slopeLimit = 60f;
			}

			// Token: 0x06001D8C RID: 7564 RVA: 0x00089F6C File Offset: 0x0008816C
			public void SetHull(HullClassification hullClassification)
			{
				HullDef hullDef = HullDef.Find(hullClassification);
				this.testCharacterController.radius = hullDef.radius;
				this.testCharacterController.height = hullDef.height;
			}

			// Token: 0x06001D8D RID: 7565 RVA: 0x00089FA2 File Offset: 0x000881A2
			public void Destroy()
			{
				UnityEngine.Object.DestroyImmediate(this.testCharacterController.gameObject);
			}

			// Token: 0x06001D8E RID: 7566 RVA: 0x00078729 File Offset: 0x00076929
			private static float DistanceXZ(Vector3 a, Vector3 b)
			{
				a.y = 0f;
				b.y = 0f;
				return Vector3.Distance(a, b);
			}

			// Token: 0x06001D8F RID: 7567 RVA: 0x00089FB4 File Offset: 0x000881B4
			public Vector3 GetGroundPosition(Vector3 footPosition)
			{
				Vector3 b = Vector3.up * (this.testCharacterController.height * 0.5f - this.testCharacterController.radius);
				Vector3 b2 = Vector3.up * (this.testCharacterController.height * 0.5f);
				Vector3 a = footPosition + b2;
				float num = this.testCharacterController.radius * 0.5f + 0.005f;
				Vector3 vector = footPosition + Vector3.up * num;
				Vector3 a2 = a + Vector3.up * num;
				RaycastHit raycastHit;
				if (Physics.CapsuleCast(a2 + b, a2 - b, this.testCharacterController.radius, Vector3.down, out raycastHit, num * 2f + 0.005f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					Vector3 b3 = raycastHit.distance * Vector3.down;
					return vector + b3;
				}
				Debug.DrawLine(vector, vector + Vector3.up * 100f, Color.red, 30f);
				return footPosition;
			}

			// Token: 0x06001D90 RID: 7568 RVA: 0x0008A0D4 File Offset: 0x000882D4
			public bool CapsuleOverlapTest(Vector3 centerOfCapsule)
			{
				Vector3 b = Vector3.up * (this.testCharacterController.height * 0.5f - this.testCharacterController.radius);
				Vector3.up * (this.testCharacterController.height * 0.5f);
				return Physics.OverlapCapsule(centerOfCapsule + b, centerOfCapsule - b, this.testCharacterController.radius, LayerIndex.world.mask | LayerIndex.defaultLayer.mask, QueryTriggerInteraction.Ignore).Length == 0;
			}

			// Token: 0x06001D91 RID: 7569 RVA: 0x0008A170 File Offset: 0x00088370
			public bool FlyTest(Vector3 startPos, Vector3 endPos, float flySpeed)
			{
				Vector3 b = Vector3.up * (this.testCharacterController.height * 0.5f - this.testCharacterController.radius);
				return !Physics.CapsuleCast(startPos + b, startPos - b, this.testCharacterController.radius, (endPos - startPos).normalized, (endPos - startPos).magnitude, LayerIndex.world.mask);
			}

			// Token: 0x06001D92 RID: 7570 RVA: 0x0008A1F8 File Offset: 0x000883F8
			public bool GroundTest(Vector3 startCenterOfCapsulePos, Vector3 endCenterOfCapsulePos, float hSpeed)
			{
				this.testCharacterController.Move(Vector3.zero);
				Vector3 a = Vector3.zero;
				float num = MapNode.MoveProbe.DistanceXZ(startCenterOfCapsulePos, endCenterOfCapsulePos);
				this.testCharacterController.transform.position = startCenterOfCapsulePos + Vector3.up;
				int num2 = Mathf.CeilToInt(num * 1.5f / hSpeed / this.testTimeStep);
				Vector3 rhs = this.testCharacterController.transform.position;
				for (int i = 0; i < num2; i++)
				{
					Vector3 vector = endCenterOfCapsulePos - this.testCharacterController.transform.position;
					if (vector.sqrMagnitude <= 0.25f)
					{
						return true;
					}
					Vector3 vector2 = vector;
					vector2.y = 0f;
					vector2.Normalize();
					a.x = vector2.x * hSpeed;
					a.z = vector2.z * hSpeed;
					a += Physics.gravity * this.testTimeStep;
					this.testCharacterController.Move(a * this.testTimeStep);
					Vector3 position = this.testCharacterController.transform.position;
					if (position == rhs)
					{
						return false;
					}
					rhs = position;
				}
				return false;
			}

			// Token: 0x06001D93 RID: 7571 RVA: 0x0008A328 File Offset: 0x00088528
			public float JumpTest(Vector3 startCenterOfCapsulePos, Vector3 endCenterOfCapsulePos, float hSpeed)
			{
				float y = Trajectory.CalculateInitialYSpeed(Trajectory.CalculateGroundTravelTime(hSpeed, MapNode.MoveProbe.DistanceXZ(startCenterOfCapsulePos, endCenterOfCapsulePos)), endCenterOfCapsulePos.y - startCenterOfCapsulePos.y);
				this.testCharacterController.Move(Vector3.zero);
				Vector3 a = endCenterOfCapsulePos - startCenterOfCapsulePos;
				a.y = 0f;
				a.Normalize();
				a *= hSpeed;
				a.y = y;
				float num = MapNode.MoveProbe.DistanceXZ(startCenterOfCapsulePos, endCenterOfCapsulePos);
				this.testCharacterController.transform.position = startCenterOfCapsulePos;
				int num2 = Mathf.CeilToInt(num * 1.5f / hSpeed / this.testTimeStep);
				float num3 = float.NegativeInfinity;
				Vector3 rhs = this.testCharacterController.transform.position;
				for (int i = 0; i < num2; i++)
				{
					Vector3 vector = endCenterOfCapsulePos - this.testCharacterController.transform.position;
					if (vector.sqrMagnitude <= 4f)
					{
						return num3 - startCenterOfCapsulePos.y;
					}
					num3 = Mathf.Max(this.testCharacterController.transform.position.y, num3);
					Vector3 vector2 = vector;
					vector2.y = 0f;
					vector2.Normalize();
					a.x = vector2.x * hSpeed;
					a.z = vector2.z * hSpeed;
					a += Physics.gravity * this.testTimeStep;
					this.testCharacterController.Move(a * this.testTimeStep);
					Vector3 position = this.testCharacterController.transform.position;
					if (position == rhs)
					{
						return 0f;
					}
					rhs = position;
				}
				return 0f;
			}

			// Token: 0x04001FD8 RID: 8152
			public CharacterController testCharacterController;

			// Token: 0x04001FD9 RID: 8153
			private float testTimeStep = 0.06666667f;
		}
	}
}
