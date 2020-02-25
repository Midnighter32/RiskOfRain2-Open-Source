using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005C8 RID: 1480
	public class HGTextMeshProUGUI : TextMeshProUGUI
	{
		// Token: 0x060022FE RID: 8958 RVA: 0x00098B29 File Offset: 0x00096D29
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Language.onCurrentLanguageChanged += HGTextMeshProUGUI.OnCurrentLanguageChanged;
			HGTextMeshProUGUI.OnCurrentLanguageChanged();
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x00098B41 File Offset: 0x00096D41
		private static void OnCurrentLanguageChanged()
		{
			HGTextMeshProUGUI.defaultLanguageFont = Resources.Load<TMP_FontAsset>(Language.GetString("DEFAULT_FONT"));
		}

		// Token: 0x06002300 RID: 8960 RVA: 0x00098B57 File Offset: 0x00096D57
		protected override void Awake()
		{
			base.Awake();
			if (this.useLanguageDefaultFont)
			{
				base.font = HGTextMeshProUGUI.defaultLanguageFont;
				base.UpdateFontAsset();
			}
		}

		// Token: 0x040020C8 RID: 8392
		public bool useLanguageDefaultFont = true;

		// Token: 0x040020C9 RID: 8393
		public static TMP_FontAsset defaultLanguageFont;
	}
}
