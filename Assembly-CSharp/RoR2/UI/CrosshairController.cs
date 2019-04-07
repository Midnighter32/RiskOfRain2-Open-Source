using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C9 RID: 1481
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(HudElement))]
	public class CrosshairController : MonoBehaviour
	{
		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06002143 RID: 8515 RVA: 0x0009C3F5 File Offset: 0x0009A5F5
		// (set) Token: 0x06002144 RID: 8516 RVA: 0x0009C3FD File Offset: 0x0009A5FD
		public RectTransform rectTransform { get; private set; }

		// Token: 0x06002145 RID: 8517 RVA: 0x0009C406 File Offset: 0x0009A606
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.hudElement = base.GetComponent<HudElement>();
			this.SetCrosshairSpread();
		}

		// Token: 0x06002146 RID: 8518 RVA: 0x0009C428 File Offset: 0x0009A628
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

		// Token: 0x06002147 RID: 8519 RVA: 0x0009C4F9 File Offset: 0x0009A6F9
		private void LateUpdate()
		{
			this.SetCrosshairSpread();
		}

		// Token: 0x040023CB RID: 9163
		private HudElement hudElement;

		// Token: 0x040023CC RID: 9164
		public CrosshairController.SpritePosition[] spriteSpreadPositions;

		// Token: 0x040023CD RID: 9165
		public RawImage[] remapSprites;

		// Token: 0x040023CE RID: 9166
		public float minSpreadAlpha;

		// Token: 0x040023CF RID: 9167
		public float maxSpreadAlpha;

		// Token: 0x040023D0 RID: 9168
		[Tooltip("The angle the crosshair represents when alpha = 1")]
		public float maxSpreadAngle;

		// Token: 0x040023D1 RID: 9169
		private MaterialPropertyBlock _propBlock;

		// Token: 0x020005CA RID: 1482
		[Serializable]
		public struct SpritePosition
		{
			// Token: 0x040023D2 RID: 9170
			public RectTransform target;

			// Token: 0x040023D3 RID: 9171
			public Vector3 zeroPosition;

			// Token: 0x040023D4 RID: 9172
			public Vector3 onePosition;
		}
	}
}
