using System;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000401 RID: 1025
	public class TimerHologramContent : MonoBehaviour
	{
		// Token: 0x060016D3 RID: 5843 RVA: 0x0006C9A4 File Offset: 0x0006ABA4
		private void FixedUpdate()
		{
			if (this.targetTextMesh)
			{
				int num = Mathf.FloorToInt(this.displayValue);
				int num2 = Mathf.FloorToInt((this.displayValue - (float)num) * 100f);
				this.targetTextMesh.text = string.Format("{0:D}.{1:D2}", num, num2);
			}
		}

		// Token: 0x040019FD RID: 6653
		public float displayValue;

		// Token: 0x040019FE RID: 6654
		public TextMeshPro targetTextMesh;
	}
}
