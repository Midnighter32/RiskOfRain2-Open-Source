using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000643 RID: 1603
	public class TypewriteTextController : MonoBehaviour
	{
		// Token: 0x060025B8 RID: 9656 RVA: 0x000A4248 File Offset: 0x000A2448
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

		// Token: 0x060025B9 RID: 9657 RVA: 0x000A42CA File Offset: 0x000A24CA
		private void Start()
		{
			this.SetupTypewriter();
		}

		// Token: 0x060025BA RID: 9658 RVA: 0x000A42D4 File Offset: 0x000A24D4
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

		// Token: 0x0400236D RID: 9069
		public float initialDelay;

		// Token: 0x0400236E RID: 9070
		public float delayBetweenKeys = 0.1f;

		// Token: 0x0400236F RID: 9071
		public float delayBetweenTexts = 1f;

		// Token: 0x04002370 RID: 9072
		public TextMeshProUGUI[] textMeshProUGui;

		// Token: 0x04002371 RID: 9073
		public string soundString;

		// Token: 0x04002372 RID: 9074
		public bool fadeOutAfterCompletion;

		// Token: 0x04002373 RID: 9075
		public float fadeOutDelay;

		// Token: 0x04002374 RID: 9076
		public float fadeOutDuration;

		// Token: 0x04002375 RID: 9077
		private string[] initialTextStrings;

		// Token: 0x04002376 RID: 9078
		private float totalStopwatch;

		// Token: 0x04002377 RID: 9079
		private float keyStopwatch;

		// Token: 0x04002378 RID: 9080
		private float fadeOutStopwatch;

		// Token: 0x04002379 RID: 9081
		private int[] keyCount;

		// Token: 0x0400237A RID: 9082
		private int textArrayIndex;

		// Token: 0x0400237B RID: 9083
		private bool done;

		// Token: 0x0400237C RID: 9084
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
