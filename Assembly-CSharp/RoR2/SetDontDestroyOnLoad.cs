using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000325 RID: 805
	public class SetDontDestroyOnLoad : MonoBehaviour
	{
		// Token: 0x060012EB RID: 4843 RVA: 0x00051319 File Offset: 0x0004F519
		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
