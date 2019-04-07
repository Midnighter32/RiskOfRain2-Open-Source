using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000410 RID: 1040
	public class UnparentOnStart : MonoBehaviour
	{
		// Token: 0x06001735 RID: 5941 RVA: 0x0006E351 File Offset: 0x0006C551
		private void Start()
		{
			base.transform.parent = null;
		}
	}
}
