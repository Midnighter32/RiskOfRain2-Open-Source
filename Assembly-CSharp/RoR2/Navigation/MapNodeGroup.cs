using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x020004DF RID: 1247
	public class MapNodeGroup : MonoBehaviour
	{
		// Token: 0x06001DCA RID: 7626 RVA: 0x0007F82C File Offset: 0x0007DA2C
		public void Clear()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x06001DCB RID: 7627 RVA: 0x0007F867 File Offset: 0x0007DA67
		public void AddNode(Vector3 position)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.position = position;
			gameObject.transform.parent = base.transform;
			gameObject.AddComponent<MapNode>();
			gameObject.name = "MapNode";
		}

		// Token: 0x06001DCC RID: 7628 RVA: 0x0007F89C File Offset: 0x0007DA9C
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

		// Token: 0x06001DCD RID: 7629 RVA: 0x0007F8EC File Offset: 0x0007DAEC
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

		// Token: 0x06001DCE RID: 7630 RVA: 0x0007F980 File Offset: 0x0007DB80
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

		// Token: 0x06001DCF RID: 7631 RVA: 0x0007FA14 File Offset: 0x0007DC14
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

		// Token: 0x04001AF4 RID: 6900
		public NodeGraph nodeGraph;

		// Token: 0x04001AF5 RID: 6901
		public Transform testPointA;

		// Token: 0x04001AF6 RID: 6902
		public Transform testPointB;

		// Token: 0x04001AF7 RID: 6903
		public HullClassification debugHullDef;

		// Token: 0x04001AF8 RID: 6904
		public MapNodeGroup.GraphType graphType;

		// Token: 0x020004E0 RID: 1248
		public enum GraphType
		{
			// Token: 0x04001AFA RID: 6906
			Ground,
			// Token: 0x04001AFB RID: 6907
			Air,
			// Token: 0x04001AFC RID: 6908
			Rail
		}
	}
}
