using System;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200022E RID: 558
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class SpawnCard : ScriptableObject
	{
		// Token: 0x06000AB3 RID: 2739 RVA: 0x00034DB0 File Offset: 0x00032FB0
		public virtual GameObject DoSpawn(Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation);
			if (this.sendOverNetwork)
			{
				NetworkServer.Spawn(gameObject);
			}
			return gameObject;
		}

		// Token: 0x04000E46 RID: 3654
		public GameObject prefab;

		// Token: 0x04000E47 RID: 3655
		public bool sendOverNetwork;

		// Token: 0x04000E48 RID: 3656
		public HullClassification hullSize;

		// Token: 0x04000E49 RID: 3657
		public MapNodeGroup.GraphType nodeGraphType;

		// Token: 0x04000E4A RID: 3658
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags requiredFlags;

		// Token: 0x04000E4B RID: 3659
		[EnumMask(typeof(NodeFlags))]
		public NodeFlags forbiddenFlags;

		// Token: 0x04000E4C RID: 3660
		public bool occupyPosition;
	}
}
