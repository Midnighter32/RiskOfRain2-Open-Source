using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026D RID: 621
	public class BoneShrink : MonoBehaviour
	{
		// Token: 0x06000BA3 RID: 2979 RVA: 0x00038E48 File Offset: 0x00037048
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x00038E56 File Offset: 0x00037056
		private void LateUpdate()
		{
			this.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		}

		// Token: 0x04000F93 RID: 3987
		private new Transform transform;
	}
}
