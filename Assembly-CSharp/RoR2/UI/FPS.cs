using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200057A RID: 1402
	public class FPS : MonoBehaviour
	{
		// Token: 0x06002155 RID: 8533 RVA: 0x000908BC File Offset: 0x0008EABC
		private void Update()
		{
			this.stopwatch += Time.unscaledDeltaTime;
			this.deltaTime += (Time.unscaledDeltaTime - this.deltaTime) * 0.1f;
			if (this.stopwatch >= 1f)
			{
				this.stopwatch -= 1f;
				float num = this.deltaTime * 1000f;
				float num2 = 1f / this.deltaTime;
				string text = string.Format("{0:0.0} ms \n{1:0.} fps", num, num2);
				this.targetText.text = text;
			}
		}

		// Token: 0x04001ED0 RID: 7888
		public TextMeshProUGUI targetText;

		// Token: 0x04001ED1 RID: 7889
		private float deltaTime;

		// Token: 0x04001ED2 RID: 7890
		private float stopwatch;

		// Token: 0x04001ED3 RID: 7891
		private const float updateTime = 1f;
	}
}
