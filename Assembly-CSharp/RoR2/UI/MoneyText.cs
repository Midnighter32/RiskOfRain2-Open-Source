using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F3 RID: 1523
	[RequireComponent(typeof(RectTransform))]
	public class MoneyText : MonoBehaviour
	{
		// Token: 0x06002404 RID: 9220 RVA: 0x0009DC50 File Offset: 0x0009BE50
		public void Update()
		{
			this.coinSoundCooldown -= Time.deltaTime;
			if (this.targetText)
			{
				if (this.updateTimer <= 0f)
				{
					int num = 0;
					if (this.displayAmount != this.targetValue)
					{
						int num2;
						num = Math.DivRem(this.targetValue - this.displayAmount, 3, out num2);
						if (num2 != 0)
						{
							num += Math.Sign(num2);
						}
						if (num > 0)
						{
							if (this.coinSoundCooldown <= 0f)
							{
								this.coinSoundCooldown = 0.025f;
								Util.PlaySound(this.sound, RoR2Application.instance.gameObject);
							}
							if (this.flashPanel)
							{
								this.flashPanel.Flash();
							}
						}
						this.displayAmount += num;
					}
					float num3 = Mathf.Min(0.5f / (float)num, 0.25f);
					this.targetText.text = TextSerialization.ToStringNumeric((double)this.displayAmount);
					this.updateTimer = num3;
				}
				this.updateTimer -= Time.deltaTime;
			}
		}

		// Token: 0x040021F9 RID: 8697
		public TextMeshProUGUI targetText;

		// Token: 0x040021FA RID: 8698
		public FlashPanel flashPanel;

		// Token: 0x040021FB RID: 8699
		private int displayAmount;

		// Token: 0x040021FC RID: 8700
		private float updateTimer;

		// Token: 0x040021FD RID: 8701
		private float coinSoundCooldown;

		// Token: 0x040021FE RID: 8702
		public int targetValue;

		// Token: 0x040021FF RID: 8703
		public string sound = "Play_UI_coin";
	}
}
