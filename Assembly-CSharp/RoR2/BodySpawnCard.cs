using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000220 RID: 544
	[CreateAssetMenu(menuName = "SpawnCards")]
	internal class BodySpawnCard : SpawnCard
	{
		// Token: 0x06000A9E RID: 2718 RVA: 0x00034998 File Offset: 0x00032B98
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation)
		{
			Vector3 position2 = position;
			position2.y += Util.GetBodyPrefabFootOffset(this.prefab);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position2, rotation);
			NetworkServer.Spawn(gameObject);
			return gameObject;
		}
	}
}
