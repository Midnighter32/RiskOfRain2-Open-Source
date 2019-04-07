using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B9 RID: 1465
	public class CarouselController : BaseSettingsControl
	{
		// Token: 0x060020CF RID: 8399 RVA: 0x0009A148 File Offset: 0x00098348
		protected override void OnUpdateControls()
		{
			string currentValue = base.GetCurrentValue();
			bool flag = false;
			for (int i = 0; i < this.choices.Length; i++)
			{
				if (this.choices[i].convarValue == currentValue)
				{
					flag = true;
					Debug.LogFormat("Matching value {0}, carousel entry index {1} detected for setting {2}", new object[]
					{
						currentValue,
						i,
						this.settingName
					});
					this.selectionIndex = i;
					this.UpdateFromSelectionIndex();
					break;
				}
			}
			if (!flag)
			{
				Debug.LogWarningFormat("Custom value of {0} detected for setting {1}", new object[]
				{
					currentValue,
					this.settingName
				});
				this.optionalText.GetComponent<LanguageTextMeshController>().token = "OPTION_CUSTOM";
			}
		}

		// Token: 0x060020D0 RID: 8400 RVA: 0x0009A1F8 File Offset: 0x000983F8
		public void MoveCarousel(int direction)
		{
			this.selectionIndex = Mathf.Clamp(this.selectionIndex + direction, 0, this.choices.Length - 1);
			this.UpdateFromSelectionIndex();
			base.SubmitSetting(this.choices[this.selectionIndex].convarValue);
		}

		// Token: 0x060020D1 RID: 8401 RVA: 0x0009A245 File Offset: 0x00098445
		public void BoolCarousel()
		{
			this.selectionIndex = ((this.selectionIndex == 0) ? 1 : 0);
			this.UpdateFromSelectionIndex();
			base.SubmitSetting(this.choices[this.selectionIndex].convarValue);
		}

		// Token: 0x060020D2 RID: 8402 RVA: 0x0009A27C File Offset: 0x0009847C
		private void UpdateFromSelectionIndex()
		{
			CarouselController.Choice choice = this.choices[this.selectionIndex];
			if (this.optionalText)
			{
				this.optionalText.GetComponent<LanguageTextMeshController>().token = choice.suboptionDisplayToken;
			}
			if (this.optionalImage)
			{
				this.optionalImage.sprite = choice.customSprite;
			}
		}

		// Token: 0x060020D3 RID: 8403 RVA: 0x0009A2DC File Offset: 0x000984DC
		private void Update()
		{
			bool active = true;
			bool active2 = true;
			if (this.selectionIndex == 0)
			{
				active = false;
			}
			else if (this.selectionIndex == this.choices.Length - 1)
			{
				active2 = false;
			}
			if (this.leftArrowButton)
			{
				this.leftArrowButton.SetActive(active);
			}
			if (this.rightArrowButton)
			{
				this.rightArrowButton.SetActive(active2);
			}
		}

		// Token: 0x04002355 RID: 9045
		public GameObject leftArrowButton;

		// Token: 0x04002356 RID: 9046
		public GameObject rightArrowButton;

		// Token: 0x04002357 RID: 9047
		public GameObject boolButton;

		// Token: 0x04002358 RID: 9048
		public Image optionalImage;

		// Token: 0x04002359 RID: 9049
		public TextMeshProUGUI optionalText;

		// Token: 0x0400235A RID: 9050
		public CarouselController.Choice[] choices;

		// Token: 0x0400235B RID: 9051
		private int selectionIndex;

		// Token: 0x020005BA RID: 1466
		[Serializable]
		public struct Choice
		{
			// Token: 0x0400235C RID: 9052
			public string suboptionDisplayToken;

			// Token: 0x0400235D RID: 9053
			public string convarValue;

			// Token: 0x0400235E RID: 9054
			public Sprite customSprite;
		}
	}
}
