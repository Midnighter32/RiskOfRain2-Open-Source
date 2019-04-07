using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000604 RID: 1540
	[RequireComponent(typeof(RectTransform))]
	public class MoneyText : MonoBehaviour
	{
		// Token: 0x06002294 RID: 8852 RVA: 0x000A3AE0 File Offset: 0x000A1CE0
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
					this.targetText.text = this.displayAmount.ToString();
					this.updateTimer = num3;
				}
				this.updateTimer -= Time.deltaTime;
			}
		}

		// Token: 0x040025B5 RID: 9653
		public TextMeshProUGUI targetText;

		// Token: 0x040025B6 RID: 9654
		public FlashPanel flashPanel;

		// Token: 0x040025B7 RID: 9655
		private int displayAmount;

		// Token: 0x040025B8 RID: 9656
		private float updateTimer;

		// Token: 0x040025B9 RID: 9657
		private float coinSoundCooldown;

		// Token: 0x040025BA RID: 9658
		public int targetValue;

		// Token: 0x040025BB RID: 9659
		public string sound = "Play_UI_coin";
	}
}
