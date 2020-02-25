using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A8 RID: 680
	public class OccupyNearbyNodes : MonoBehaviour
	{
		// Token: 0x06000F86 RID: 3974 RVA: 0x00044412 File Offset: 0x00042612
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.position, this.radius);
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x00044434 File Offset: 0x00042634
		private void OnEnable()
		{
			OccupyNearbyNodes.instancesList.Add(this);
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x00044441 File Offset: 0x00042641
		private void OnDisable()
		{
			OccupyNearbyNodes.instancesList.Remove(this);
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0004444F File Offset: 0x0004264F
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			SceneDirector.onPrePopulateSceneServer += OccupyNearbyNodes.OnSceneDirectorPrePopulateSceneServer;
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x00044464 File Offset: 0x00042664
		private static void OnSceneDirectorPrePopulateSceneServer(SceneDirector sceneDirector)
		{
			DirectorCore instance = DirectorCore.instance;
			NodeGraph groundNodeGraph = SceneInfo.instance.GetNodeGraph(MapNodeGroup.GraphType.Ground);
			foreach (NodeGraph.NodeIndex nodeIndex in OccupyNearbyNodes.instancesList.SelectMany((OccupyNearbyNodes v) => groundNodeGraph.FindNodesInRange(v.transform.position, 0f, v.radius, HullMask.None)).Distinct<NodeGraph.NodeIndex>().ToArray<NodeGraph.NodeIndex>())
			{
				instance.AddOccupiedNode(groundNodeGraph, nodeIndex);
			}
		}

		// Token: 0x04000EF4 RID: 3828
		public float radius = 5f;

		// Token: 0x04000EF5 RID: 3829
		private static readonly List<OccupyNearbyNodes> instancesList = new List<OccupyNearbyNodes>();
	}
}
