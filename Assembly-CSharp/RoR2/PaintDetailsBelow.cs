using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037F RID: 895
	public class PaintDetailsBelow : MonoBehaviour
	{
		// Token: 0x060012A9 RID: 4777 RVA: 0x0005B815 File Offset: 0x00059A15
		public void OnEnable()
		{
			PaintDetailsBelow.painterList.Add(this);
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x0005B822 File Offset: 0x00059A22
		public void OnDisable()
		{
			PaintDetailsBelow.painterList.Remove(this);
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x0005B830 File Offset: 0x00059A30
		public static List<PaintDetailsBelow> GetPainters()
		{
			return PaintDetailsBelow.painterList;
		}

		// Token: 0x0400166C RID: 5740
		[Tooltip("Influence radius in world coordinates")]
		public float influenceOuter = 2f;

		// Token: 0x0400166D RID: 5741
		public float influenceInner = 1f;

		// Token: 0x0400166E RID: 5742
		[Tooltip("Which detail layer")]
		public int layer;

		// Token: 0x0400166F RID: 5743
		[Tooltip("Density, from 0-1")]
		public float density = 0.5f;

		// Token: 0x04001670 RID: 5744
		public float densityPower = 1f;

		// Token: 0x04001671 RID: 5745
		private int buffer = 1;

		// Token: 0x04001672 RID: 5746
		private float steepnessMax = 30f;

		// Token: 0x04001673 RID: 5747
		private static List<PaintDetailsBelow> painterList = new List<PaintDetailsBelow>();
	}
}
