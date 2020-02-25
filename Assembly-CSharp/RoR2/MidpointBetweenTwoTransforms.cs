using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000284 RID: 644
	public class MidpointBetweenTwoTransforms : MonoBehaviour
	{
		// Token: 0x06000E3C RID: 3644 RVA: 0x0003F94A File Offset: 0x0003DB4A
		public void Update()
		{
			base.transform.position = Vector3.Lerp(this.transform1.position, this.transform2.position, 0.5f);
		}

		// Token: 0x04000E3C RID: 3644
		public Transform transform1;

		// Token: 0x04000E3D RID: 3645
		public Transform transform2;
	}
}
