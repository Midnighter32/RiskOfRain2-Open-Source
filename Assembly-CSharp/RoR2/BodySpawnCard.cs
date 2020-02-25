using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000100 RID: 256
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class BodySpawnCard : SpawnCard
	{
		// Token: 0x060004E4 RID: 1252 RVA: 0x00013AA4 File Offset: 0x00011CA4
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation, DirectorSpawnRequest directorSpawnRequest)
		{
			Vector3 position2 = position;
			position2.y += Util.GetBodyPrefabFootOffset(this.prefab);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position2, rotation);
			NetworkServer.Spawn(gameObject);
			return gameObject;
		}
	}
}
