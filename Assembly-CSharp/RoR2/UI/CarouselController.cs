using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000593 RID: 1427
	public class CarouselController : BaseSettingsControl
	{
		// Token: 0x060021F1 RID: 8689 RVA: 0x00092934 File Offset: 0x00090B34
		protected override void OnUpdateControls()
		{
			string currentValue = base.GetCurrentValue();
			bool flag = false;
			for (int i = 0; i < this.choices.Length; i++)
			{
				if (this.choices[i].convarValue == currentValue)
				{
					flag = true;
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

		// Token: 0x060021F2 RID: 8690 RVA: 0x000929BC File Offset: 0x00090BBC
		public void MoveCarousel(int direction)
		{
			this.selectionIndex = Mathf.Clamp(this.selectionIndex + direction, 0, this.choices.Length - 1);
			this.UpdateFromSelectionIndex();
			base.SubmitSetting(this.choices[this.selectionIndex].convarValue);
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x00092A09 File Offset: 0x00090C09
		public void BoolCarousel()
		{
			this.selectionIndex = ((this.selectionIndex == 0) ? 1 : 0);
			this.UpdateFromSelectionIndex();
			base.SubmitSetting(this.choices[this.selectionIndex].convarValue);
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x00092A40 File Offset: 0x00090C40
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

		// Token: 0x060021F5 RID: 8693 RVA: 0x00092AA0 File Offset: 0x00090CA0
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

		// Token: 0x04001F4B RID: 8011
		public GameObject leftArrowButton;

		// Token: 0x04001F4C RID: 8012
		public GameObject rightArrowButton;

		// Token: 0x04001F4D RID: 8013
		public GameObject boolButton;

		// Token: 0x04001F4E RID: 8014
		public Image optionalImage;

		// Token: 0x04001F4F RID: 8015
		public TextMeshProUGUI optionalText;

		// Token: 0x04001F50 RID: 8016
		public CarouselController.Choice[] choices;

		// Token: 0x04001F51 RID: 8017
		private int selectionIndex;

		// Token: 0x02000594 RID: 1428
		[Serializable]
		public struct Choice
		{
			// Token: 0x04001F52 RID: 8018
			public string suboptionDisplayToken;

			// Token: 0x04001F53 RID: 8019
			public string convarValue;

			// Token: 0x04001F54 RID: 8020
			public Sprite customSprite;
		}
	}
}
