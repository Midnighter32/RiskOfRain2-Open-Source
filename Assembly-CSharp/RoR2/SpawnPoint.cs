using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200033D RID: 829
	public class SpawnPoint : MonoBehaviour
	{
		// Token: 0x17000258 RID: 600
		// (get) Token: 0x060013B9 RID: 5049 RVA: 0x0005467D File Offset: 0x0005287D
		public static ReadOnlyCollection<SpawnPoint> readOnlyInstancesList
		{
			get
			{
				return SpawnPoint._readOnlyInstancesList;
			}
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x00054684 File Offset: 0x00052884
		private void OnEnable()
		{
			SpawnPoint.instancesList.Add(this);
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x00054694 File Offset: 0x00052894
		public static SpawnPoint ConsumeSpawnPoint()
		{
			if (SpawnPoint.instancesList.Count == 0)
			{
				return null;
			}
			SpawnPoint spawnPoint = null;
			for (int i = 0; i < SpawnPoint.readOnlyInstancesList.Count; i++)
			{
				if (!SpawnPoint.readOnlyInstancesList[i].consumed)
				{
					spawnPoint = SpawnPoint.readOnlyInstancesList[i];
					SpawnPoint.readOnlyInstancesList[i].consumed = true;
					break;
				}
			}
			if (!spawnPoint)
			{
				for (int j = 0; j < SpawnPoint.readOnlyInstancesList.Count; j++)
				{
					SpawnPoint.readOnlyInstancesList[j].consumed = false;
				}
				spawnPoint = SpawnPoint.readOnlyInstancesList[0];
			}
			return spawnPoint;
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x00054732 File Offset: 0x00052932
		private void OnDisable()
		{
			SpawnPoint.instancesList.Remove(this);
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x00054740 File Offset: 0x00052940
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			SpawnPoint.prefab = Resources.Load<GameObject>("Prefabs/SpawnPoint");
		}

		// Token: 0x060013BE RID: 5054 RVA: 0x00054754 File Offset: 0x00052954
		public unsafe static void AddSpawnPoint(NodeGraph nodeGraph, NodeGraph.NodeIndex nodeIndex, Xoroshiro128Plus rng)
		{
			Vector3 vector;
			nodeGraph.GetNodePosition(nodeIndex, out vector);
			NodeGraph.LinkIndex[] activeNodeLinks = nodeGraph.GetActiveNodeLinks(nodeIndex);
			Quaternion rotation;
			if (activeNodeLinks.Length != 0)
			{
				NodeGraph.LinkIndex linkIndex = *rng.NextElementUniform<NodeGraph.LinkIndex>(activeNodeLinks);
				Vector3 a;
				nodeGraph.GetNodePosition(nodeGraph.GetLinkEndNode(linkIndex), out a);
				rotation = Util.QuaternionSafeLookRotation(a - vector);
			}
			else
			{
				rotation = Quaternion.Euler(0f, rng.nextNormalizedFloat * 360f, 0f);
			}
			SpawnPoint.AddSpawnPoint(vector, rotation);
		}

		// Token: 0x060013BF RID: 5055 RVA: 0x000547C7 File Offset: 0x000529C7
		public static void AddSpawnPoint(Vector3 position, Quaternion rotation)
		{
			UnityEngine.Object.Instantiate<GameObject>(SpawnPoint.prefab, position, rotation);
		}

		// Token: 0x0400128B RID: 4747
		private static List<SpawnPoint> instancesList = new List<SpawnPoint>();

		// Token: 0x0400128C RID: 4748
		private static ReadOnlyCollection<SpawnPoint> _readOnlyInstancesList = new ReadOnlyCollection<SpawnPoint>(SpawnPoint.instancesList);

		// Token: 0x0400128D RID: 4749
		[Tooltip("Flagged when a player spawns on this position, to stop overlapping spawn positions")]
		public bool consumed;

		// Token: 0x0400128E RID: 4750
		private static GameObject prefab;
	}
}
