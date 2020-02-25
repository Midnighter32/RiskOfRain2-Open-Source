using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002A0 RID: 672
	public class NetworkSpawnOnStart : MonoBehaviour
	{
		// Token: 0x06000F08 RID: 3848 RVA: 0x0004267D File Offset: 0x0004087D
		private void Start()
		{
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(base.gameObject);
			}
		}
	}
}
