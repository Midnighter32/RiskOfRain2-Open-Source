using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200025D RID: 605
	public class InstantiatePrefabOnStart : MonoBehaviour
	{
		// Token: 0x06000D36 RID: 3382 RVA: 0x0003B634 File Offset: 0x00039834
		public void Start()
		{
			if (!this.networkedPrefab || NetworkServer.active)
			{
				Vector3 position = this.targetTransform ? this.targetTransform.position : Vector3.zero;
				Quaternion rotation = this.copyTargetRotation ? this.targetTransform.rotation : Quaternion.identity;
				Transform parent = this.parentToTarget ? this.targetTransform : null;
				GameObject obj = UnityEngine.Object.Instantiate<GameObject>(this.prefab, position, rotation, parent);
				if (this.networkedPrefab)
				{
					NetworkServer.Spawn(obj);
				}
			}
		}

		// Token: 0x04000D71 RID: 3441
		[Tooltip("The prefab to instantiate.")]
		public GameObject prefab;

		// Token: 0x04000D72 RID: 3442
		[Tooltip("The object upon which the prefab will be positioned.")]
		public Transform targetTransform;

		// Token: 0x04000D73 RID: 3443
		[Tooltip("The transform upon which to instantiate the prefab.")]
		public bool copyTargetRotation;

		// Token: 0x04000D74 RID: 3444
		[Tooltip("Whether or not to parent the instantiated prefab to the specified transform.")]
		public bool parentToTarget;

		// Token: 0x04000D75 RID: 3445
		[Tooltip("Whether or not this is a networked prefab. If so, this will only run on the server, and will be spawned over the network.")]
		public bool networkedPrefab;
	}
}
