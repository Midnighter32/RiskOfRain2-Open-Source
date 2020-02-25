using System;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000111 RID: 273
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class SpawnCard : ScriptableObject
	{
		// Token: 0x06000515 RID: 1301 RVA: 0x00014494 File Offset: 0x00012694
		public virtual GameObject DoSpawn(Vector3 position, Quaternion rotation, DirectorSpawnRequest spawnRequest)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation);
			if (this.sendOverNetwork)
			{
				NetworkServer.Spawn(gameObject);
			}
			return gameObject;
		}

		// Token: 0x04000500 RID: 1280
		public GameObject prefab;

		// Token: 0x04000501 RID: 1281
		public bool sendOverNetwork;

		// Token: 0x04000502 RID: 1282
		public HullClassification hullSize;

		// Token: 0x04000503 RID: 1283
		public MapNodeGroup.GraphType nodeGraphType;

		// Token: 0x04000504 RID: 1284
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags requiredFlags;

		// Token: 0x04000505 RID: 1285
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags forbiddenFlags;

		// Token: 0x04000506 RID: 1286
		public int directorCreditCost;

		// Token: 0x04000507 RID: 1287
		public bool occupyPosition;
	}
}
