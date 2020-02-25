using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000369 RID: 873
	public class UnparentOnStart : MonoBehaviour
	{
		// Token: 0x0600153C RID: 5436 RVA: 0x0005A729 File Offset: 0x00058929
		private void Start()
		{
			base.transform.parent = null;
		}
	}
}
