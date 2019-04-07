using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036F RID: 879
	public class NetworkSpawnOnStart : MonoBehaviour
	{
		// Token: 0x06001227 RID: 4647 RVA: 0x000595D9 File Offset: 0x000577D9
		private void Start()
		{
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(base.gameObject);
			}
		}
	}
}
