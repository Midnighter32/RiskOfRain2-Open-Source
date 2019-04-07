using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005FB RID: 1531
	[RequireComponent(typeof(RectTransform))]
	public class LevelText : MonoBehaviour
	{
		// Token: 0x0600224A RID: 8778 RVA: 0x000A20B8 File Offset: 0x000A02B8
		private void SetDisplayData(uint newDisplayData)
		{
			if (this.displayData == newDisplayData)
			{
				return;
			}
			this.displayData = newDisplayData;
			uint value = this.displayData;
			LevelText.sharedStringBuilder.Clear();
			LevelText.sharedStringBuilder.AppendUint(value, 0u, uint.MaxValue);
			this.targetText.SetText(LevelText.sharedStringBuilder);
		}

		// Token: 0x0600224B RID: 8779 RVA: 0x000A2105 File Offset: 0x000A0305
		private void Update()
		{
			if (this.source && TeamManager.instance)
			{
				this.SetDisplayData(TeamManager.instance.GetTeamLevel(this.source.teamIndex));
			}
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x000A213B File Offset: 0x000A033B
		private void OnValidate()
		{
			if (!this.targetText)
			{
				Debug.LogError("targetText must be assigned.");
			}
		}

		// Token: 0x04002565 RID: 9573
		public CharacterMaster source;

		// Token: 0x04002566 RID: 9574
		public TextMeshProUGUI targetText;

		// Token: 0x04002567 RID: 9575
		private uint displayData;

		// Token: 0x04002568 RID: 9576
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
