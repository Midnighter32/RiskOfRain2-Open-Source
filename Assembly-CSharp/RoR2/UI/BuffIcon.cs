using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B7 RID: 1463
	[RequireComponent(typeof(RectTransform))]
	public class BuffIcon : MonoBehaviour
	{
		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x060020C5 RID: 8389 RVA: 0x00099FC5 File Offset: 0x000981C5
		// (set) Token: 0x060020C6 RID: 8390 RVA: 0x00099FCD File Offset: 0x000981CD
		public RectTransform rectTransform { get; private set; }

		// Token: 0x060020C7 RID: 8391 RVA: 0x00099FD6 File Offset: 0x000981D6
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.UpdateIcon();
		}

		// Token: 0x060020C8 RID: 8392 RVA: 0x00099FEC File Offset: 0x000981EC
		public void Flash()
		{
			BuffDef buffDef = BuffCatalog.GetBuffDef(this.buffIndex);
			this.iconImage.color = Color.white;
			this.iconImage.CrossFadeColor(buffDef.buffColor, 0.25f, true, false);
		}

		// Token: 0x060020C9 RID: 8393 RVA: 0x0009A030 File Offset: 0x00098230
		public void UpdateIcon()
		{
			BuffDef buffDef = BuffCatalog.GetBuffDef(this.buffIndex);
			if (buffDef == null)
			{
				this.iconImage.sprite = null;
				return;
			}
			this.iconImage.sprite = Resources.Load<Sprite>(buffDef.iconPath);
			this.iconImage.color = buffDef.buffColor;
			if (buffDef.canStack)
			{
				BuffIcon.sharedStringBuilder.Clear();
				BuffIcon.sharedStringBuilder.Append("x");
				BuffIcon.sharedStringBuilder.AppendInt(this.buffCount, 0u, uint.MaxValue);
				this.stackCount.enabled = true;
				this.stackCount.SetText(BuffIcon.sharedStringBuilder);
				return;
			}
			this.stackCount.enabled = false;
		}

		// Token: 0x060020CA RID: 8394 RVA: 0x0009A0E1 File Offset: 0x000982E1
		private void Update()
		{
			if (this.lastBuffIndex != this.buffIndex)
			{
				this.lastBuffIndex = this.buffIndex;
			}
		}

		// Token: 0x0400234B RID: 9035
		private BuffIndex lastBuffIndex;

		// Token: 0x0400234C RID: 9036
		public BuffIndex buffIndex = BuffIndex.None;

		// Token: 0x0400234D RID: 9037
		public Image iconImage;

		// Token: 0x0400234E RID: 9038
		public TextMeshProUGUI stackCount;

		// Token: 0x0400234F RID: 9039
		public int buffCount;

		// Token: 0x04002350 RID: 9040
		private float stopwatch;

		// Token: 0x04002351 RID: 9041
		private const float flashDuration = 0.25f;

		// Token: 0x04002352 RID: 9042
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
