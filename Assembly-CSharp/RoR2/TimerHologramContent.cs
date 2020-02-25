using System;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000359 RID: 857
	public class TimerHologramContent : MonoBehaviour
	{
		// Token: 0x060014D5 RID: 5333 RVA: 0x00058CAC File Offset: 0x00056EAC
		private void FixedUpdate()
		{
			if (this.targetTextMesh)
			{
				int num = Mathf.FloorToInt(this.displayValue);
				int num2 = Mathf.FloorToInt((this.displayValue - (float)num) * 100f);
				this.targetTextMesh.text = string.Format("{0:D}.{1:D2}", num, num2);
			}
		}

		// Token: 0x04001364 RID: 4964
		public float displayValue;

		// Token: 0x04001365 RID: 4965
		public TextMeshPro targetTextMesh;
	}
}
