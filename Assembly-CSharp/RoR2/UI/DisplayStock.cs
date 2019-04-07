using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D7 RID: 1495
	public class DisplayStock : MonoBehaviour
	{
		// Token: 0x06002183 RID: 8579 RVA: 0x0009DA7F File Offset: 0x0009BC7F
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x06002184 RID: 8580 RVA: 0x0009DA90 File Offset: 0x0009BC90
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

		// Token: 0x0400242F RID: 9263
		public SkillSlot skillSlot;

		// Token: 0x04002430 RID: 9264
		public Image[] stockImages;

		// Token: 0x04002431 RID: 9265
		public Sprite fullStockSprite;

		// Token: 0x04002432 RID: 9266
		public Color fullStockColor;

		// Token: 0x04002433 RID: 9267
		public Sprite emptyStockSprite;

		// Token: 0x04002434 RID: 9268
		public Color emptyStockColor;

		// Token: 0x04002435 RID: 9269
		private HudElement hudElement;

		// Token: 0x04002436 RID: 9270
		private SkillLocator skillLocator;
	}
}
