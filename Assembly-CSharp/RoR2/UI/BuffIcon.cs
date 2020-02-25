using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000591 RID: 1425
	[RequireComponent(typeof(RectTransform))]
	public class BuffIcon : MonoBehaviour
	{
		// Token: 0x17000396 RID: 918
		// (get) Token: 0x060021E7 RID: 8679 RVA: 0x000927AC File Offset: 0x000909AC
		// (set) Token: 0x060021E8 RID: 8680 RVA: 0x000927B4 File Offset: 0x000909B4
		public RectTransform rectTransform { get; private set; }

		// Token: 0x060021E9 RID: 8681 RVA: 0x000927BD File Offset: 0x000909BD
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.UpdateIcon();
		}

		// Token: 0x060021EA RID: 8682 RVA: 0x000927D4 File Offset: 0x000909D4
		public void Flash()
		{
			BuffDef buffDef = BuffCatalog.GetBuffDef(this.buffIndex);
			this.iconImage.color = Color.white;
			this.iconImage.CrossFadeColor(buffDef.buffColor, 0.25f, true, false);
		}

		// Token: 0x060021EB RID: 8683 RVA: 0x00092818 File Offset: 0x00090A18
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
				BuffIcon.sharedStringBuilder.AppendInt(this.buffCount, 0U, uint.MaxValue);
				this.stackCount.enabled = true;
				this.stackCount.SetText(BuffIcon.sharedStringBuilder);
				return;
			}
			this.stackCount.enabled = false;
		}

		// Token: 0x060021EC RID: 8684 RVA: 0x000928CA File Offset: 0x00090ACA
		private void Update()
		{
			if (this.lastBuffIndex != this.buffIndex)
			{
				this.lastBuffIndex = this.buffIndex;
			}
		}

		// Token: 0x04001F41 RID: 8001
		private BuffIndex lastBuffIndex;

		// Token: 0x04001F42 RID: 8002
		public BuffIndex buffIndex = BuffIndex.None;

		// Token: 0x04001F43 RID: 8003
		public Image iconImage;

		// Token: 0x04001F44 RID: 8004
		public TextMeshProUGUI stackCount;

		// Token: 0x04001F45 RID: 8005
		public int buffCount;

		// Token: 0x04001F46 RID: 8006
		private float stopwatch;

		// Token: 0x04001F47 RID: 8007
		private const float flashDuration = 0.25f;

		// Token: 0x04001F48 RID: 8008
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
