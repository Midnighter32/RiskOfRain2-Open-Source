using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200010C RID: 268
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class MultiCharacterSpawnCard : CharacterSpawnCard
	{
		// Token: 0x06000505 RID: 1285 RVA: 0x00014327 File Offset: 0x00012527
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation, DirectorSpawnRequest directorSpawnRequest)
		{
			this.prefab = this.masterPrefabs[(int)(directorSpawnRequest.rng.nextNormalizedFloat * (float)this.masterPrefabs.Length)];
			return base.DoSpawn(position, rotation, directorSpawnRequest);
		}

		// Token: 0x040004D7 RID: 1239
		public GameObject[] masterPrefabs;
	}
}
