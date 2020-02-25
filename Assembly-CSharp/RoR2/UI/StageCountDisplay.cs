using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000635 RID: 1589
	public class StageCountDisplay : MonoBehaviour
	{
		// Token: 0x06002573 RID: 9587 RVA: 0x000A2F64 File Offset: 0x000A1164
		private void Update()
		{
			string text = "-";
			if (Run.instance)
			{
				text = (Run.instance.stageClearCount + 1).ToString();
			}
			this.text.text = Language.GetStringFormatted("STAGE_COUNT_FORMAT", new object[]
			{
				text
			});
		}

		// Token: 0x0400232D RID: 9005
		public TextMeshProUGUI text;
	}
}
