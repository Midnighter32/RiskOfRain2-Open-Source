using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005A7 RID: 1447
	public class FPS : MonoBehaviour
	{
		// Token: 0x06002065 RID: 8293 RVA: 0x00098D04 File Offset: 0x00096F04
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

		// Token: 0x040022FD RID: 8957
		public TextMeshProUGUI targetText;

		// Token: 0x040022FE RID: 8958
		private float deltaTime;

		// Token: 0x040022FF RID: 8959
		private float stopwatch;

		// Token: 0x04002300 RID: 8960
		private const float updateTime = 1f;
	}
}
