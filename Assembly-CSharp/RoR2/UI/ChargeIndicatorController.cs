using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200059D RID: 1437
	public class ChargeIndicatorController : MonoBehaviour
	{
		// Token: 0x06002222 RID: 8738 RVA: 0x00093B38 File Offset: 0x00091D38
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

		// Token: 0x04001F83 RID: 8067
		public SpriteRenderer[] iconSprites;

		// Token: 0x04001F84 RID: 8068
		public TextMeshPro chargingText;

		// Token: 0x04001F85 RID: 8069
		public Color spriteBaseColor;

		// Token: 0x04001F86 RID: 8070
		public Color spriteFlashColor;

		// Token: 0x04001F87 RID: 8071
		public Color spriteChargingColor;

		// Token: 0x04001F88 RID: 8072
		public Color spriteChargedColor;

		// Token: 0x04001F89 RID: 8073
		public Color textBaseColor;

		// Token: 0x04001F8A RID: 8074
		public Color textChargingColor;

		// Token: 0x04001F8B RID: 8075
		public bool isCharging;

		// Token: 0x04001F8C RID: 8076
		public bool isCharged;

		// Token: 0x04001F8D RID: 8077
		public bool disableTextWhenNotCharging;

		// Token: 0x04001F8E RID: 8078
		public bool disableTextWhenCharged;

		// Token: 0x04001F8F RID: 8079
		public bool flashWhenNotCharging;

		// Token: 0x04001F90 RID: 8080
		public float flashFrequency;

		// Token: 0x04001F91 RID: 8081
		private float flashStopwatch;
	}
}
