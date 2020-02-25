using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000574 RID: 1396
	[CreateAssetMenu]
	public class UISkinData : ScriptableObject
	{
		// Token: 0x04001EA3 RID: 7843
		[Header("Main Panel Style")]
		public UISkinData.PanelStyle mainPanelStyle;

		// Token: 0x04001EA4 RID: 7844
		[Header("Header Style")]
		public UISkinData.PanelStyle headerPanelStyle;

		// Token: 0x04001EA5 RID: 7845
		public UISkinData.TextStyle headerTextStyle;

		// Token: 0x04001EA6 RID: 7846
		[Header("Detail Style")]
		public UISkinData.PanelStyle detailPanelStyle;

		// Token: 0x04001EA7 RID: 7847
		public UISkinData.TextStyle detailTextStyle;

		// Token: 0x04001EA8 RID: 7848
		[Header("Body Style")]
		public UISkinData.TextStyle bodyTextStyle;

		// Token: 0x04001EA9 RID: 7849
		[Header("Button Style")]
		public UISkinData.ButtonStyle buttonStyle;

		// Token: 0x04001EAA RID: 7850
		[Header("Scroll Rect Style")]
		public UISkinData.ScrollRectStyle scrollRectStyle;

		// Token: 0x02000575 RID: 1397
		[Serializable]
		public struct TextStyle
		{
			// Token: 0x0600214D RID: 8525 RVA: 0x00090244 File Offset: 0x0008E444
			public void Apply(TextMeshProUGUI label, bool useAlignment = true)
			{
				HGTextMeshProUGUI hgtextMeshProUGUI;
				if (label.font != this.font && ((hgtextMeshProUGUI = (label as HGTextMeshProUGUI)) == null || !hgtextMeshProUGUI.useLanguageDefaultFont))
				{
					label.font = this.font;
				}
				if (label.fontSize != this.fontSize)
				{
					label.fontSize = this.fontSize;
				}
				if (label.color != this.color)
				{
					label.color = this.color;
				}
				if (useAlignment && label.alignment != this.alignment)
				{
					label.alignment = this.alignment;
				}
			}

			// Token: 0x04001EAB RID: 7851
			public TMP_FontAsset font;

			// Token: 0x04001EAC RID: 7852
			public float fontSize;

			// Token: 0x04001EAD RID: 7853
			public TextAlignmentOptions alignment;

			// Token: 0x04001EAE RID: 7854
			public Color color;
		}

		// Token: 0x02000576 RID: 1398
		[Serializable]
		public struct PanelStyle
		{
			// Token: 0x0600214E RID: 8526 RVA: 0x000902D8 File Offset: 0x0008E4D8
			public void Apply(Image image)
			{
				image.material = this.material;
				image.sprite = this.sprite;
				image.color = this.color;
			}

			// Token: 0x04001EAF RID: 7855
			public Material material;

			// Token: 0x04001EB0 RID: 7856
			public Sprite sprite;

			// Token: 0x04001EB1 RID: 7857
			public Color color;
		}

		// Token: 0x02000577 RID: 1399
		[Serializable]
		public struct ButtonStyle
		{
			// Token: 0x04001EB2 RID: 7858
			public Material material;

			// Token: 0x04001EB3 RID: 7859
			public Sprite sprite;

			// Token: 0x04001EB4 RID: 7860
			public ColorBlock colors;

			// Token: 0x04001EB5 RID: 7861
			public UISkinData.TextStyle interactableTextStyle;

			// Token: 0x04001EB6 RID: 7862
			public UISkinData.TextStyle disabledTextStyle;

			// Token: 0x04001EB7 RID: 7863
			public float recommendedWidth;

			// Token: 0x04001EB8 RID: 7864
			public float recommendedHeight;
		}

		// Token: 0x02000578 RID: 1400
		[Serializable]
		public struct ScrollRectStyle
		{
			// Token: 0x04001EB9 RID: 7865
			[FormerlySerializedAs("viewportPanelStyle")]
			public UISkinData.PanelStyle backgroundPanelStyle;

			// Token: 0x04001EBA RID: 7866
			public UISkinData.PanelStyle scrollbarBackgroundStyle;

			// Token: 0x04001EBB RID: 7867
			public ColorBlock scrollbarHandleColors;

			// Token: 0x04001EBC RID: 7868
			public Sprite scrollbarHandleImage;
		}
	}
}
