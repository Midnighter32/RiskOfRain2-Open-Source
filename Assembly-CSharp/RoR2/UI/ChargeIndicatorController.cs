using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005C1 RID: 1473
	public class ChargeIndicatorController : MonoBehaviour
	{
		// Token: 0x060020F8 RID: 8440 RVA: 0x0009AF84 File Offset: 0x00099184
		private void Update()
		{
			Color color = this.spriteBaseColor;
			Color color2 = this.textBaseColor;
			if (!this.isCharged)
			{
				if (this.flashWhenNotCharging)
				{
					this.flashStopwatch += Time.deltaTime;
					color = (((int)(this.flashStopwatch * this.flashFrequency) % 2 == 0) ? this.spriteFlashColor : this.spriteBaseColor);
				}
				if (this.isCharging)
				{
					color = this.spriteChargingColor;
					color2 = this.textChargingColor;
				}
				if (this.disableTextWhenNotCharging)
				{
					this.chargingText.enabled = this.isCharging;
				}
				else
				{
					this.chargingText.enabled = true;
				}
			}
			else
			{
				color = this.spriteChargedColor;
				if (this.disableTextWhenCharged)
				{
					this.chargingText.enabled = false;
				}
			}
			SpriteRenderer[] array = this.iconSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = color;
			}
			this.chargingText.color = color2;
		}

		// Token: 0x04002386 RID: 9094
		public SpriteRenderer[] iconSprites;

		// Token: 0x04002387 RID: 9095
		public TextMeshPro chargingText;

		// Token: 0x04002388 RID: 9096
		public Color spriteBaseColor;

		// Token: 0x04002389 RID: 9097
		public Color spriteFlashColor;

		// Token: 0x0400238A RID: 9098
		public Color spriteChargingColor;

		// Token: 0x0400238B RID: 9099
		public Color spriteChargedColor;

		// Token: 0x0400238C RID: 9100
		public Color textBaseColor;

		// Token: 0x0400238D RID: 9101
		public Color textChargingColor;

		// Token: 0x0400238E RID: 9102
		public bool isCharging;

		// Token: 0x0400238F RID: 9103
		public bool isCharged;

		// Token: 0x04002390 RID: 9104
		public bool disableTextWhenNotCharging;

		// Token: 0x04002391 RID: 9105
		public bool disableTextWhenCharged;

		// Token: 0x04002392 RID: 9106
		public bool flashWhenNotCharging;

		// Token: 0x04002393 RID: 9107
		public float flashFrequency;

		// Token: 0x04002394 RID: 9108
		private float flashStopwatch;
	}
}
