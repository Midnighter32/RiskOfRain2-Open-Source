using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x02000525 RID: 1317
	public class MapNodeGroup : MonoBehaviour
	{
		// Token: 0x06001D97 RID: 7575 RVA: 0x0008A4F0 File Offset: 0x000886F0
		public void Clear()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x0008A52B File Offset: 0x0008872B
		public void AddNode(Vector3 position)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.position = position;
			gameObject.transform.parent = base.transform;
			gameObject.AddComponent<MapNode>();
			gameObject.name = "MapNode";
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x0008A560 File Offset: 0x00088760
		public List<MapNode> GetNodes()
		{
			List<MapNode> list = new List<MapNode>();
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				MapNode component = base.transform.GetChild(i).GetComponent<MapNode>();
				if (component)
				{
					list.Add(component);
				}
			}
			return list;
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x0008A5B0 File Offset: 0x000887B0
		public void UpdateNoCeilingMasks()
		{
			int num = 0;
			foreach (MapNode mapNode in this.GetNodes())
			{
				mapNode.flags &= ~NodeFlags.NoCeiling;
				if (mapNode.TestNoCeiling())
				{
					num++;
					mapNode.flags |= NodeFlags.NoCeiling;
				}
			}
			Debug.LogFormat("{0} successful ceiling masks baked.", new object[]
			{
				num
			});
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x0008A644 File Offset: 0x00088844
		public void UpdateTeleporterMasks()
		{
			int num = 0;
			foreach (MapNode mapNode in this.GetNodes())
			{
				mapNode.flags &= ~NodeFlags.TeleporterOK;
				if (mapNode.TestTeleporterOK())
				{
					num++;
					mapNode.flags |= NodeFlags.TeleporterOK;
				}
			}
			Debug.LogFormat("{0} successful teleporter masks baked.", new object[]
			{
				num
			});
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x0008A6D8 File Offset: 0x000888D8
		public void Bake(NodeGraph nodeGraph)
		{
			List<MapNode> nodes = this.GetNodes();
			ReadOnlyCollection<MapNode> readOnlyCollection = nodes.AsReadOnly();
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].BuildLinks(readOnlyCollection, this.graphType);
			}
			List<SerializableBitArray> list = new List<SerializableBitArray>();
			for (int j = 0; j < nodes.Count; j++)
			{
				MapNode mapNode = nodes[j];
				SerializableBitArray serializableBitArray = new SerializableBitArray(nodes.Count);
				for (int k = 0; k < nodes.Count; k++)
				{
					MapNode other = nodes[k];
					serializableBitArray[k] = mapNode.TestLineOfSight(other);
				}
				list.Add(serializableBitArray);
			}
			nodeGraph.SetNodes(readOnlyCollection, list.AsReadOnly());
		}

		// Token: 0x04001FDB RID: 8155
		public NodeGraph nodeGraph;

		// Token: 0x04001FDC RID: 8156
		public Transform testPointA;

		// Token: 0x04001FDD RID: 8157
		public Transform testPointB;

		// Token: 0x04001FDE RID: 8158
		public HullClassification debugHullDef;

		// Token: 0x04001FDF RID: 8159
		public MapNodeGroup.GraphType graphType;

		// Token: 0x02000526 RID: 1318
		public enum GraphType
		{
			// Token: 0x04001FE1 RID: 8161
			Ground,
			// Token: 0x04001FE2 RID: 8162
			Air,
			// Token: 0x04001FE3 RID: 8163
			Rail
		}
	}
}
