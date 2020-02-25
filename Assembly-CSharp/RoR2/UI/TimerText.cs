using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x0200063F RID: 1599
	[RequireComponent(typeof(RectTransform))]
	public class TimerText : MonoBehaviour
	{
		// Token: 0x0600259F RID: 9631 RVA: 0x000A3BA0 File Offset: 0x000A1DA0
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
			TimerText.sharedStringBuilder.Append("<mspace=0.5em>");
			TimerText.sharedStringBuilder.AppendInt(num, 2U, uint.MaxValue);
			TimerText.sharedStringBuilder.Append(":");
			TimerText.sharedStringBuilder.AppendUint((uint)num2, 2U, 2U);
			TimerText.sharedStringBuilder.Append("</mspace><voffset=0.4em><size=40%><mspace=0.5em>:");
			TimerText.sharedStringBuilder.AppendUint((uint)value, 2U, 2U);
			TimerText.sharedStringBuilder.Append("</size></voffset></mspace>");
			this.targetLabel.SetText(TimerText.sharedStringBuilder);
		}

		// Token: 0x060025A0 RID: 9632 RVA: 0x000A3C74 File Offset: 0x000A1E74
		private void Update()
		{
			float displayData = 0f;
			Run instance = Run.instance;
			if (instance)
			{
				displayData = instance.GetRunStopwatch();
			}
			this.SetDisplayData(displayData);
		}

		// Token: 0x060025A1 RID: 9633 RVA: 0x000A3CA3 File Offset: 0x000A1EA3
		private void OnValidate()
		{
			if (!this.targetLabel)
			{
				Debug.LogErrorFormat(this, "TimerText does not specify a target label.", Array.Empty<object>());
			}
		}

		// Token: 0x0400234F RID: 9039
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x04002350 RID: 9040
		[FormerlySerializedAs("targetText")]
		public TextMeshProUGUI targetLabel;

		// Token: 0x04002351 RID: 9041
		public bool showTimerTutorial;

		// Token: 0x04002352 RID: 9042
		private float currentDisplayData;
	}
}
