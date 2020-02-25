using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B6 RID: 1462
	public class DisplayStock : MonoBehaviour
	{
		// Token: 0x060022B3 RID: 8883 RVA: 0x000969F3 File Offset: 0x00094BF3
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x00096A04 File Offset: 0x00094C04
		private void Update()
		{
			if (this.hudElement.targetCharacterBody)
			{
				if (!this.skillLocator)
				{
					this.skillLocator = this.hudElement.targetCharacterBody.GetComponent<SkillLocator>();
				}
				if (this.skillLocator)
				{
					GenericSkill skill = this.skillLocator.GetSkill(this.skillSlot);
					if (skill)
					{
						for (int i = 0; i < this.stockImages.Length; i++)
						{
							if (skill.stock > i)
							{
								this.stockImages[i].sprite = this.fullStockSprite;
								this.stockImages[i].color = this.fullStockColor;
							}
							else
							{
								this.stockImages[i].sprite = this.emptyStockSprite;
								this.stockImages[i].color = this.emptyStockColor;
							}
						}
					}
				}
			}
		}

		// Token: 0x0400203E RID: 8254
		public SkillSlot skillSlot;

		// Token: 0x0400203F RID: 8255
		public Image[] stockImages;

		// Token: 0x04002040 RID: 8256
		public Sprite fullStockSprite;

		// Token: 0x04002041 RID: 8257
		public Color fullStockColor;

		// Token: 0x04002042 RID: 8258
		public Sprite emptyStockSprite;

		// Token: 0x04002043 RID: 8259
		public Color emptyStockColor;

		// Token: 0x04002044 RID: 8260
		private HudElement hudElement;

		// Token: 0x04002045 RID: 8261
		private SkillLocator skillLocator;
	}
}
