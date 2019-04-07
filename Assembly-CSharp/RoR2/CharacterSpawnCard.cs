using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000222 RID: 546
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class CharacterSpawnCard : SpawnCard
	{
		// Token: 0x06000AA1 RID: 2721 RVA: 0x00034A31 File Offset: 0x00032C31
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation);
			NetworkServer.Spawn(gameObject);
			gameObject.GetComponent<CharacterMaster>().Respawn(position, rotation);
			return gameObject;
		}

		// Token: 0x04000E09 RID: 3593
		public bool noElites;
	}
}
