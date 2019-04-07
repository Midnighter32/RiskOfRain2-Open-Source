using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035B RID: 859
	public class MidpointBetweenTwoTransforms : MonoBehaviour
	{
		// Token: 0x060011A7 RID: 4519 RVA: 0x000578FA File Offset: 0x00055AFA
		public void Update()
		{
			base.transform.position = Vector3.Lerp(this.transform1.position, this.transform2.position, 0.5f);
		}

		// Token: 0x040015C4 RID: 5572
		public Transform transform1;

		// Token: 0x040015C5 RID: 5573
		public Transform transform2;
	}
}
