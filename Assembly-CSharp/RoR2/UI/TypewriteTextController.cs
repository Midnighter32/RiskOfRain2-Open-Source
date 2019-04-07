using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200064E RID: 1614
	public class TypewriteTextController : MonoBehaviour
	{
		// Token: 0x06002419 RID: 9241 RVA: 0x000A9780 File Offset: 0x000A7980
		private void SetupTypewriter()
		{
			this.totalStopwatch = -this.initialDelay;
			this.keyStopwatch = -this.initialDelay;
			this.textArrayIndex = 0;
			this.initialTextStrings = new string[this.textMeshProUGui.Length];
			this.keyCount = new int[this.textMeshProUGui.Length];
			for (int i = 0; i < this.textMeshProUGui.Length; i++)
			{
				this.initialTextStrings[i] = this.textMeshProUGui[i].text;
			}
			this.Update();
		}

		// Token: 0x0600241A RID: 9242 RVA: 0x000A9802 File Offset: 0x000A7A02
		private void Start()
		{
			this.SetupTypewriter();
		}

		// Token: 0x0600241B RID: 9243 RVA: 0x000A980C File Offset: 0x000A7A0C
		private void Update()
		{
			if (this.done)
			{
				if (this.fadeOutAfterCompletion)
				{
					this.fadeOutStopwatch += Time.deltaTime;
					float t = (this.fadeOutStopwatch - this.fadeOutDelay) / this.fadeOutDuration;
					for (int i = 0; i < this.textMeshProUGui.Length; i++)
					{
						Color color = this.textMeshProUGui[i].color;
						this.textMeshProUGui[i].color = new Color(color.r, color.g, color.b, Mathf.Lerp(1f, 0f, t));
					}
					if (this.fadeOutStopwatch >= this.fadeOutDelay + this.fadeOutDuration)
					{
						base.gameObject.SetActive(false);
					}
				}
				return;
			}
			this.totalStopwatch += Time.deltaTime;
			this.keyStopwatch += Time.deltaTime;
			if (this.keyStopwatch >= this.delayBetweenKeys)
			{
				this.keyStopwatch -= this.delayBetweenKeys;
				this.keyCount[this.textArrayIndex]++;
				if (this.soundString.Length > 0)
				{
					Util.PlaySound(this.soundString, RoR2Application.instance.gameObject);
				}
				if (this.keyCount[this.textArrayIndex] > this.initialTextStrings[this.textArrayIndex].Length)
				{
					if (this.textArrayIndex + 1 == this.textMeshProUGui.Length)
					{
						this.done = true;
					}
					else
					{
						this.textArrayIndex++;
						this.keyCount[this.textArrayIndex] = 0;
						this.keyStopwatch -= this.delayBetweenTexts;
					}
				}
			}
			for (int j = 0; j < this.textMeshProUGui.Length; j++)
			{
				if (this.keyCount[j] <= 0)
				{
					this.textMeshProUGui[j].text = "";
				}
				else
				{
					TypewriteTextController.sharedStringBuilder.Clear();
					TypewriteTextController.sharedStringBuilder.Append(this.initialTextStrings[j], 0, Mathf.Min(this.keyCount[j], this.initialTextStrings[j].Length));
					this.textMeshProUGui[j].SetText(TypewriteTextController.sharedStringBuilder);
				}
			}
		}

		// Token: 0x04002708 RID: 9992
		public float initialDelay;

		// Token: 0x04002709 RID: 9993
		public float delayBetweenKeys = 0.1f;

		// Token: 0x0400270A RID: 9994
		public float delayBetweenTexts = 1f;

		// Token: 0x0400270B RID: 9995
		public TextMeshProUGUI[] textMeshProUGui;

		// Token: 0x0400270C RID: 9996
		public string soundString;

		// Token: 0x0400270D RID: 9997
		public bool fadeOutAfterCompletion;

		// Token: 0x0400270E RID: 9998
		public float fadeOutDelay;

		// Token: 0x0400270F RID: 9999
		public float fadeOutDuration;

		// Token: 0x04002710 RID: 10000
		private string[] initialTextStrings;

		// Token: 0x04002711 RID: 10001
		private float totalStopwatch;

		// Token: 0x04002712 RID: 10002
		private float keyStopwatch;

		// Token: 0x04002713 RID: 10003
		private float fadeOutStopwatch;

		// Token: 0x04002714 RID: 10004
		private int[] keyCount;

		// Token: 0x04002715 RID: 10005
		private int textArrayIndex;

		// Token: 0x04002716 RID: 10006
		private bool done;

		// Token: 0x04002717 RID: 10007
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
