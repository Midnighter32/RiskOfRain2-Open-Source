using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.LogBook
{
	// Token: 0x02000675 RID: 1653
	public class LogBookPage : MonoBehaviour
	{
		// Token: 0x060026BF RID: 9919 RVA: 0x000A8F28 File Offset: 0x000A7128
		public void SetEntry(UserProfile userProfile, Entry entry)
		{
			PageBuilder pageBuilder = this.pageBuilder;
			if (pageBuilder != null)
			{
				pageBuilder.Destroy();
			}
			this.pageBuilder = new PageBuilder();
			this.pageBuilder.container = this.contentContainer;
			this.pageBuilder.entry = entry;
			this.pageBuilder.userProfile = userProfile;
			Action<PageBuilder> addEntries = entry.addEntries;
			if (addEntries != null)
			{
				addEntries(this.pageBuilder);
			}
			this.iconImage.texture = entry.iconTexture;
			this.titleText.text = Language.GetString(entry.nameToken);
			this.categoryText.text = Language.GetString(entry.categoryTypeToken);
			this.modelPanel.modelPrefab = entry.modelPrefab;
		}

		// Token: 0x040024A8 RID: 9384
		private int currentEntryIndex;

		// Token: 0x040024A9 RID: 9385
		public RawImage iconImage;

		// Token: 0x040024AA RID: 9386
		public ModelPanel modelPanel;

		// Token: 0x040024AB RID: 9387
		public TextMeshProUGUI titleText;

		// Token: 0x040024AC RID: 9388
		public TextMeshProUGUI categoryText;

		// Token: 0x040024AD RID: 9389
		public TextMeshProUGUI pageNumberText;

		// Token: 0x040024AE RID: 9390
		public RectTransform contentContainer;

		// Token: 0x040024AF RID: 9391
		private PageBuilder pageBuilder;
	}
}
