using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E9 RID: 1001
	public class SpawnPoint : MonoBehaviour
	{
		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060015CB RID: 5579 RVA: 0x0006887D File Offset: 0x00066A7D
		public static ReadOnlyCollection<SpawnPoint> readOnlyInstancesList
		{
			get
			{
				return SpawnPoint._readOnlyInstancesList;
			}
		}

		// Token: 0x060015CC RID: 5580 RVA: 0x00068884 File Offset: 0x00066A84
		private void OnEnable()
		{
			SpawnPoint.instancesList.Add(this);
		}

		// Token: 0x060015CD RID: 5581 RVA: 0x00068894 File Offset: 0x00066A94
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

		// Token: 0x060015CE RID: 5582 RVA: 0x00068932 File Offset: 0x00066B32
		private void OnDisable()
		{
			SpawnPoint.instancesList.Remove(this);
		}

		// Token: 0x04001935 RID: 6453
		private static List<SpawnPoint> instancesList = new List<SpawnPoint>();

		// Token: 0x04001936 RID: 6454
		private static ReadOnlyCollection<SpawnPoint> _readOnlyInstancesList = new ReadOnlyCollection<SpawnPoint>(SpawnPoint.instancesList);

		// Token: 0x04001937 RID: 6455
		[Tooltip("Flagged when a player spawns on this position, to stop overlapping spawn positions")]
		public bool consumed;
	}
}
