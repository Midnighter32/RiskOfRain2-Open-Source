using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000105 RID: 261
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class InteractableSpawnCard : SpawnCard
	{
		// Token: 0x060004EC RID: 1260 RVA: 0x00013C14 File Offset: 0x00011E14
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation, DirectorSpawnRequest directorSpawnRequest)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation);
			Transform transform = gameObject.transform;
			RaycastHit raycastHit;
			if (this.orientToFloor && Physics.Raycast(new Ray(position + gameObject.transform.up * 3f, -transform.up), out raycastHit, 9f, LayerIndex.world.mask))
			{
				transform.up = raycastHit.normal;
			}
			transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f), Space.Self);
			if (this.slightlyRandomizeOrientation)
			{
				transform.Translate(Vector3.down * 0.3f, Space.Self);
				transform.rotation *= Quaternion.Euler(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-30f, 30f));
			}
			NetworkServer.Spawn(gameObject);
			return gameObject;
		}

		// Token: 0x040004B7 RID: 1207
		public bool orientToFloor;

		// Token: 0x040004B8 RID: 1208
		public bool slightlyRandomizeOrientation;

		// Token: 0x040004B9 RID: 1209
		private const float raycastLength = 6f;

		// Token: 0x040004BA RID: 1210
		private const float floorOffset = 3f;
	}
}
