using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000339 RID: 825
	public class InstantiatePrefabOnStart : MonoBehaviour
	{
		// Token: 0x060010E1 RID: 4321 RVA: 0x000543C0 File Offset: 0x000525C0
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

		// Token: 0x04001522 RID: 5410
		[Tooltip("The prefab to instantiate.")]
		public GameObject prefab;

		// Token: 0x04001523 RID: 5411
		[Tooltip("The object upon which the prefab will be positioned.")]
		public Transform targetTransform;

		// Token: 0x04001524 RID: 5412
		[Tooltip("The transform upon which to instantiate the prefab.")]
		public bool copyTargetRotation;

		// Token: 0x04001525 RID: 5413
		[Tooltip("Whether or not to parent the instantiated prefab to the specified transform.")]
		public bool parentToTarget;

		// Token: 0x04001526 RID: 5414
		[Tooltip("Whether or not this is a networked prefab. If so, this will only run on the server, and will be spawned over the network.")]
		public bool networkedPrefab;
	}
}
