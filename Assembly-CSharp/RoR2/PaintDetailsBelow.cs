using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002B7 RID: 695
	public class PaintDetailsBelow : MonoBehaviour
	{
		// Token: 0x06000FB5 RID: 4021 RVA: 0x00044E79 File Offset: 0x00043079
		public void OnEnable()
		{
			PaintDetailsBelow.painterList.Add(this);
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x00044E86 File Offset: 0x00043086
		public void OnDisable()
		{
			PaintDetailsBelow.painterList.Remove(this);
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x00044E94 File Offset: 0x00043094
		public static List<PaintDetailsBelow> GetPainters()
		{
			return PaintDetailsBelow.painterList;
		}

		// Token: 0x04000F29 RID: 3881
		[Tooltip("Influence radius in world coordinates")]
		public float influenceOuter = 2f;

		// Token: 0x04000F2A RID: 3882
		public float influenceInner = 1f;

		// Token: 0x04000F2B RID: 3883
		[Tooltip("Which detail layer")]
		public int layer;

		// Token: 0x04000F2C RID: 3884
		[Tooltip("Density, from 0-1")]
		public float density = 0.5f;

		// Token: 0x04000F2D RID: 3885
		public float densityPower = 1f;

		// Token: 0x04000F2E RID: 3886
		private int buffer = 1;

		// Token: 0x04000F2F RID: 3887
		private float steepnessMax = 30f;

		// Token: 0x04000F30 RID: 3888
		private static List<PaintDetailsBelow> painterList = new List<PaintDetailsBelow>();
	}
}
