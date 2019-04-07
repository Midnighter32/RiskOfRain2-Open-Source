using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.LogBook
{
	// Token: 0x02000680 RID: 1664
	public class LogBookPage : MonoBehaviour
	{
		// Token: 0x0600251A RID: 9498 RVA: 0x000AE41C File Offset: 0x000AC61C
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

		// Token: 0x04002841 RID: 10305
		private int currentEntryIndex;

		// Token: 0x04002842 RID: 10306
		public RawImage iconImage;

		// Token: 0x04002843 RID: 10307
		public ModelPanel modelPanel;

		// Token: 0x04002844 RID: 10308
		public TextMeshProUGUI titleText;

		// Token: 0x04002845 RID: 10309
		public TextMeshProUGUI categoryText;

		// Token: 0x04002846 RID: 10310
		public TextMeshProUGUI pageNumberText;

		// Token: 0x04002847 RID: 10311
		public RectTransform contentContainer;

		// Token: 0x04002848 RID: 10312
		private PageBuilder pageBuilder;
	}
}
