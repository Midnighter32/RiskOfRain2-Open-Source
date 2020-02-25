using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005A7 RID: 1447
	[RequireComponent(typeof(HudElement))]
	[RequireComponent(typeof(RectTransform))]
	public class CrosshairController : MonoBehaviour
	{
		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06002272 RID: 8818 RVA: 0x000951DE File Offset: 0x000933DE
		// (set) Token: 0x06002273 RID: 8819 RVA: 0x000951E6 File Offset: 0x000933E6
		public RectTransform rectTransform { get; private set; }

		// Token: 0x06002274 RID: 8820 RVA: 0x000951EF File Offset: 0x000933EF
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.hudElement = base.GetComponent<HudElement>();
			this.SetCrosshairSpread();
			this.SetSkillStockDisplays();
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x00095218 File Offset: 0x00093418
		private void SetCrosshairSpread()
		{
			float num = 0f;
			if (this.hudElement.targetCharacterBody)
			{
				num = this.hudElement.targetCharacterBody.spreadBloomAngle;
			}
			for (int i = 0; i < this.spriteSpreadPositions.Length; i++)
			{
				CrosshairController.SpritePosition spritePosition = this.spriteSpreadPositions[i];
				spritePosition.target.localPosition = Vector3.Lerp(spritePosition.zeroPosition, spritePosition.onePosition, num / this.maxSpreadAngle);
			}
			for (int j = 0; j < this.remapSprites.Length; j++)
			{
				this.remapSprites[j].color = new Color(1f, 1f, 1f, Util.Remap(num / this.maxSpreadAngle, 0f, 1f, this.minSpreadAlpha, this.maxSpreadAlpha));
			}
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x000952EC File Offset: 0x000934EC
		private void SetSkillStockDisplays()
		{
			if (this.hudElement.targetCharacterBody)
			{
				SkillLocator component = this.hudElement.targetCharacterBody.GetComponent<SkillLocator>();
				for (int i = 0; i < this.skillStockSpriteDisplays.Length; i++)
				{
					bool active = false;
					CrosshairController.SkillStockSpriteDisplay skillStockSpriteDisplay = this.skillStockSpriteDisplays[i];
					GenericSkill skill = component.GetSkill(skillStockSpriteDisplay.skillSlot);
					if (skill && skill.stock >= skillStockSpriteDisplay.minimumStockCountToBeValid && skill.stock <= skillStockSpriteDisplay.maximumStockCountToBeValid)
					{
						active = true;
					}
					skillStockSpriteDisplay.target.SetActive(active);
				}
			}
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x00095381 File Offset: 0x00093581
		private void LateUpdate()
		{
			this.SetCrosshairSpread();
			this.SetSkillStockDisplays();
		}

		// Token: 0x04001FD3 RID: 8147
		private HudElement hudElement;

		// Token: 0x04001FD4 RID: 8148
		public CrosshairController.SpritePosition[] spriteSpreadPositions;

		// Token: 0x04001FD5 RID: 8149
		public CrosshairController.SkillStockSpriteDisplay[] skillStockSpriteDisplays;

		// Token: 0x04001FD6 RID: 8150
		public RawImage[] remapSprites;

		// Token: 0x04001FD7 RID: 8151
		public float minSpreadAlpha;

		// Token: 0x04001FD8 RID: 8152
		public float maxSpreadAlpha;

		// Token: 0x04001FD9 RID: 8153
		[Tooltip("The angle the crosshair represents when alpha = 1")]
		public float maxSpreadAngle;

		// Token: 0x04001FDA RID: 8154
		private MaterialPropertyBlock _propBlock;

		// Token: 0x020005A8 RID: 1448
		[Serializable]
		public struct SpritePosition
		{
			// Token: 0x04001FDB RID: 8155
			public RectTransform target;

			// Token: 0x04001FDC RID: 8156
			public Vector3 zeroPosition;

			// Token: 0x04001FDD RID: 8157
			public Vector3 onePosition;
		}

		// Token: 0x020005A9 RID: 1449
		[Serializable]
		public struct SkillStockSpriteDisplay
		{
			// Token: 0x04001FDE RID: 8158
			public GameObject target;

			// Token: 0x04001FDF RID: 8159
			public SkillSlot skillSlot;

			// Token: 0x04001FE0 RID: 8160
			public int minimumStockCountToBeValid;

			// Token: 0x04001FE1 RID: 8161
			public int maximumStockCountToBeValid;
		}
	}
}
