using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005E0 RID: 1504
	[RequireComponent(typeof(RectTransform))]
	public class LevelText : MonoBehaviour
	{
		// Token: 0x0600238F RID: 9103 RVA: 0x0009B4CC File Offset: 0x000996CC
		private void SetDisplayData(uint newDisplayData)
		{
			if (this.displayData == newDisplayData)
			{
				return;
			}
			this.displayData = newDisplayData;
			uint value = this.displayData;
			LevelText.sharedStringBuilder.Clear();
			LevelText.sharedStringBuilder.AppendUint(value, 0U, uint.MaxValue);
			this.targetText.SetText(LevelText.sharedStringBuilder);
		}

		// Token: 0x06002390 RID: 9104 RVA: 0x0009B51A File Offset: 0x0009971A
		private void Update()
		{
			if (this.source && TeamManager.instance)
			{
				this.SetDisplayData(TeamManager.instance.GetTeamLevel(this.source.teamIndex));
			}
		}

		// Token: 0x06002391 RID: 9105 RVA: 0x0009B550 File Offset: 0x00099750
		private void OnValidate()
		{
			if (!this.targetText)
			{
				Debug.LogError("targetText must be assigned.");
			}
		}

		// Token: 0x04002184 RID: 8580
		public CharacterMaster source;

		// Token: 0x04002185 RID: 8581
		public TextMeshProUGUI targetText;

		// Token: 0x04002186 RID: 8582
		private uint displayData;

		// Token: 0x04002187 RID: 8583
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
