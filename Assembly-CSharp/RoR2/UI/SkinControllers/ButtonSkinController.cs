using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x0200065F RID: 1631
	[RequireComponent(typeof(Button))]
	public class ButtonSkinController : BaseSkinController
	{
		// Token: 0x0600246C RID: 9324 RVA: 0x000AAC4E File Offset: 0x000A8E4E
		protected new void Awake()
		{
			this.button = base.GetComponent<Button>();
			this.layoutElement = base.GetComponent<LayoutElement>();
			base.Awake();
		}

		// Token: 0x0600246D RID: 9325 RVA: 0x000AAC6E File Offset: 0x000A8E6E
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onUpdate += ButtonSkinController.StaticUpdate;
		}

		// Token: 0x0600246E RID: 9326 RVA: 0x000AAC81 File Offset: 0x000A8E81
		private void OnEnable()
		{
			ButtonSkinController.instancesList.Add(this);
		}

		// Token: 0x0600246F RID: 9327 RVA: 0x000AAC8E File Offset: 0x000A8E8E
		private void OnDisable()
		{
			ButtonSkinController.instancesList.Remove(this);
		}

		// Token: 0x06002470 RID: 9328 RVA: 0x000AAC9C File Offset: 0x000A8E9C
		private static void StaticUpdate()
		{
			foreach (ButtonSkinController buttonSkinController in ButtonSkinController.instancesList)
			{
				buttonSkinController.UpdateLabelStyle(ref buttonSkinController.skinData.buttonStyle);
			}
		}

		// Token: 0x06002471 RID: 9329 RVA: 0x000AACF8 File Offset: 0x000A8EF8
		private void UpdateLabelStyle(ref UISkinData.ButtonStyle buttonStyle)
		{
			if (this.useRecommendedLabel)
			{
				TextMeshProUGUI componentInChildren = this.button.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren)
				{
					if (this.button.interactable)
					{
						buttonStyle.interactableTextStyle.Apply(componentInChildren, this.useRecommendedAlignment);
						return;
					}
					buttonStyle.disabledTextStyle.Apply(componentInChildren, this.useRecommendedAlignment);
				}
			}
		}

		// Token: 0x06002472 RID: 9330 RVA: 0x000AAD53 File Offset: 0x000A8F53
		protected override void OnSkinUI()
		{
			this.ApplyButtonStyle(ref this.skinData.buttonStyle);
		}

		// Token: 0x06002473 RID: 9331 RVA: 0x000AAD68 File Offset: 0x000A8F68
		private void ApplyButtonStyle(ref UISkinData.ButtonStyle buttonStyle)
		{
			if (this.useRecommendedMaterial)
			{
				this.button.image.material = buttonStyle.material;
			}
			this.button.colors = buttonStyle.colors;
			if (this.useRecommendedImage)
			{
				this.button.image.sprite = buttonStyle.sprite;
			}
			if (this.useRecommendedButtonWidth)
			{
				((RectTransform)base.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonStyle.recommendedWidth);
			}
			if (this.useRecommendedButtonHeight)
			{
				((RectTransform)base.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonStyle.recommendedHeight);
			}
			if (this.layoutElement)
			{
				if (this.useRecommendedButtonWidth)
				{
					this.layoutElement.preferredWidth = buttonStyle.recommendedWidth;
				}
				if (this.useRecommendedButtonHeight)
				{
					this.layoutElement.preferredHeight = buttonStyle.recommendedHeight;
				}
			}
			this.UpdateLabelStyle(ref buttonStyle);
		}

		// Token: 0x04002768 RID: 10088
		private static readonly List<ButtonSkinController> instancesList = new List<ButtonSkinController>();

		// Token: 0x04002769 RID: 10089
		private Button button;

		// Token: 0x0400276A RID: 10090
		public bool useRecommendedButtonWidth = true;

		// Token: 0x0400276B RID: 10091
		public bool useRecommendedButtonHeight = true;

		// Token: 0x0400276C RID: 10092
		public bool useRecommendedImage = true;

		// Token: 0x0400276D RID: 10093
		public bool useRecommendedMaterial = true;

		// Token: 0x0400276E RID: 10094
		public bool useRecommendedAlignment = true;

		// Token: 0x0400276F RID: 10095
		public bool useRecommendedLabel = true;

		// Token: 0x04002770 RID: 10096
		private LayoutElement layoutElement;
	}
}
