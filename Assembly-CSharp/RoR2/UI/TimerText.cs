using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x0200064A RID: 1610
	[RequireComponent(typeof(RectTransform))]
	public class TimerText : MonoBehaviour
	{
		// Token: 0x06002400 RID: 9216 RVA: 0x000A90DC File Offset: 0x000A72DC
		private void SetDisplayData(float newDisplayData)
		{
			if (newDisplayData == this.currentDisplayData)
			{
				return;
			}
			this.currentDisplayData = newDisplayData;
			int num = Mathf.FloorToInt(newDisplayData * 0.016666668f);
			int num2 = (int)newDisplayData - num * 60;
			int value = Mathf.FloorToInt((newDisplayData - (float)num2 - (float)num * 60f) * 100f);
			TimerText.sharedStringBuilder.Clear();
			TimerText.sharedStringBuilder.Append("<mspace=2.0em>");
			TimerText.sharedStringBuilder.AppendInt(num, 2u, uint.MaxValue);
			TimerText.sharedStringBuilder.Append("</mspace>:<mspace=2.0em>");
			TimerText.sharedStringBuilder.AppendUint((uint)num2, 2u, 2u);
			TimerText.sharedStringBuilder.Append("</mspace><voffset=0.4em><size=40%><mspace=2.0em>:");
			TimerText.sharedStringBuilder.AppendUint((uint)value, 2u, 2u);
			TimerText.sharedStringBuilder.Append("</size></voffset></mspace>");
			this.targetLabel.SetText(TimerText.sharedStringBuilder);
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x000A91AC File Offset: 0x000A73AC
		private void Update()
		{
			float displayData = 0f;
			Run instance = Run.instance;
			if (instance)
			{
				displayData = instance.time;
			}
			this.SetDisplayData(displayData);
		}

		// Token: 0x06002402 RID: 9218 RVA: 0x000A91DB File Offset: 0x000A73DB
		private void OnValidate()
		{
			if (!this.targetLabel)
			{
				Debug.LogErrorFormat(this, "TimerText does not specify a target label.", Array.Empty<object>());
			}
		}

		// Token: 0x040026EA RID: 9962
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x040026EB RID: 9963
		[FormerlySerializedAs("targetText")]
		public TextMeshProUGUI targetLabel;

		// Token: 0x040026EC RID: 9964
		public bool showTimerTutorial;

		// Token: 0x040026ED RID: 9965
		private float currentDisplayData;
	}
}
