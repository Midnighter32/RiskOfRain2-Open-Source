using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000654 RID: 1620
	[RequireComponent(typeof(Button))]
	public class ButtonSkinController : BaseSkinController
	{
		// Token: 0x06002610 RID: 9744 RVA: 0x000A578A File Offset: 0x000A398A
		protected new void Awake()
		{
			this.button = base.GetComponent<Button>();
			this.layoutElement = base.GetComponent<LayoutElement>();
			base.Awake();
		}

		// Token: 0x06002611 RID: 9745 RVA: 0x000A57AA File Offset: 0x000A39AA
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onUpdate += ButtonSkinController.StaticUpdate;
		}

		// Token: 0x06002612 RID: 9746 RVA: 0x000A57BD File Offset: 0x000A39BD
		private void OnEnable()
		{
			ButtonSkinController.instancesList.Add(this);
		}

		// Token: 0x06002613 RID: 9747 RVA: 0x000A57CA File Offset: 0x000A39CA
		private void OnDisable()
		{
			ButtonSkinController.instancesList.Remove(this);
		}

		// Token: 0x06002614 RID: 9748 RVA: 0x000A57D8 File Offset: 0x000A39D8
		private static void StaticUpdate()
		{
			foreach (ButtonSkinController buttonSkinController in ButtonSkinController.instancesList)
			{
				buttonSkinController.UpdateLabelStyle(ref buttonSkinController.skinData.buttonStyle);
			}
		}

		// Token: 0x06002615 RID: 9749 RVA: 0x000A5834 File Offset: 0x000A3A34
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

		// Token: 0x06002616 RID: 9750 RVA: 0x000A588F File Offset: 0x000A3A8F
		protected override void OnSkinUI()
		{
			this.ApplyButtonStyle(ref this.skinData.buttonStyle);
		}

		// Token: 0x06002617 RID: 9751 RVA: 0x000A58A4 File Offset: 0x000A3AA4
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

		// Token: 0x040023CF RID: 9167
		private static readonly List<ButtonSkinController> instancesList = new List<ButtonSkinController>();

		// Token: 0x040023D0 RID: 9168
		private Button button;

		// Token: 0x040023D1 RID: 9169
		public bool useRecommendedButtonWidth = true;

		// Token: 0x040023D2 RID: 9170
		public bool useRecommendedButtonHeight = true;

		// Token: 0x040023D3 RID: 9171
		public bool useRecommendedImage = true;

		// Token: 0x040023D4 RID: 9172
		public bool useRecommendedMaterial = true;

		// Token: 0x040023D5 RID: 9173
		public bool useRecommendedAlignment = true;

		// Token: 0x040023D6 RID: 9174
		public bool useRecommendedLabel = true;

		// Token: 0x040023D7 RID: 9175
		private LayoutElement layoutElement;
	}
}
