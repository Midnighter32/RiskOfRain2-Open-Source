using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000163 RID: 355
	public class BoneShrink : MonoBehaviour
	{
		// Token: 0x06000681 RID: 1665 RVA: 0x0001AB48 File Offset: 0x00018D48
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x0001AB56 File Offset: 0x00018D56
		private void LateUpdate()
		{
			this.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		}

		// Token: 0x040006DB RID: 1755
		private new Transform transform;
	}
}
