using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D4 RID: 980
	public class SetDontDestroyOnLoad : MonoBehaviour
	{
		// Token: 0x0600153C RID: 5436 RVA: 0x0005992A File Offset: 0x00057B2A
		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
