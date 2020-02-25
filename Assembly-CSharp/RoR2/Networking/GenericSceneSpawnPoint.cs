using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200053E RID: 1342
	public class GenericSceneSpawnPoint : MonoBehaviour
	{
		// Token: 0x06001FAB RID: 8107 RVA: 0x00089A7C File Offset: 0x00087C7C
		private void Start()
		{
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(this.networkedObjectPrefab, base.transform.position, base.transform.rotation));
				base.gameObject.SetActive(false);
				return;
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x04001D65 RID: 7525
		public GameObject networkedObjectPrefab;
	}
}
