using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000575 RID: 1397
	public class GenericSceneSpawnPoint : MonoBehaviour
	{
		// Token: 0x06001F1C RID: 7964 RVA: 0x00092D5C File Offset: 0x00090F5C
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

		// Token: 0x040021D3 RID: 8659
		public GameObject networkedObjectPrefab;
	}
}
