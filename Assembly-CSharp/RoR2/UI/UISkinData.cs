using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005A1 RID: 1441
	[CreateAssetMenu]
	public class UISkinData : ScriptableObject
	{
		// Token: 0x040022D0 RID: 8912
		[Header("Main Panel Style")]
		public UISkinData.PanelStyle mainPanelStyle;

		// Token: 0x040022D1 RID: 8913
		[Header("Header Style")]
		public UISkinData.PanelStyle headerPanelStyle;

		// Token: 0x040022D2 RID: 8914
		public UISkinData.TextStyle headerTextStyle;

		// Token: 0x040022D3 RID: 8915
		[Header("Detail Style")]
		public UISkinData.PanelStyle detailPanelStyle;

		// Token: 0x040022D4 RID: 8916
		public UISkinData.TextStyle detailTextStyle;

		// Token: 0x040022D5 RID: 8917
		[Header("Body Style")]
		public UISkinData.TextStyle bodyTextStyle;

		// Token: 0x040022D6 RID: 8918
		[Header("Button Style")]
		public UISkinData.ButtonStyle buttonStyle;

		// Token: 0x040022D7 RID: 8919
		[Header("Scroll Rect Style")]
		public UISkinData.ScrollRectStyle scrollRectStyle;

		// Token: 0x020005A2 RID: 1442
		[Serializable]
		public struct TextStyle
		{
			// Token: 0x0600205C RID: 8284 RVA: 0x00098688 File Offset: 0x00096888
			public void Apply(TextMeshProUGUI label, bool useAlignment = true)
			{
				if (label.font != this.font)
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

			// Token: 0x040022D8 RID: 8920
			public TMP_FontAsset font;

			// Token: 0x040022D9 RID: 8921
			public float fontSize;

			// Token: 0x040022DA RID: 8922
			public TextAlignmentOptions alignment;

			// Token: 0x040022DB RID: 8923
			public Color color;
		}

		// Token: 0x020005A3 RID: 1443
		[Serializable]
		public struct PanelStyle
		{
			// Token: 0x0600205D RID: 8285 RVA: 0x0009870A File Offset: 0x0009690A
			public void Apply(Image image)
			{
				image.material = this.material;
				image.sprite = this.sprite;
				image.color = this.color;
			}

			// Token: 0x040022DC RID: 8924
			public Material material;

			// Token: 0x040022DD RID: 8925
			public Sprite sprite;

			// Token: 0x040022DE RID: 8926
			public Color color;
		}

		// Token: 0x020005A4 RID: 1444
		[Serializable]
		public struct ButtonStyle
		{
			// Token: 0x040022DF RID: 8927
			public Material material;

			// Token: 0x040022E0 RID: 8928
			public Sprite sprite;

			// Token: 0x040022E1 RID: 8929
			public ColorBlock colors;

			// Token: 0x040022E2 RID: 8930
			public UISkinData.TextStyle interactableTextStyle;

			// Token: 0x040022E3 RID: 8931
			public UISkinData.TextStyle disabledTextStyle;

			// Token: 0x040022E4 RID: 8932
			public float recommendedWidth;

			// Token: 0x040022E5 RID: 8933
			public float recommendedHeight;
		}

		// Token: 0x020005A5 RID: 1445
		[Serializable]
		public struct ScrollRectStyle
		{
			// Token: 0x040022E6 RID: 8934
			[FormerlySerializedAs("viewportPanelStyle")]
			public UISkinData.PanelStyle backgroundPanelStyle;

			// Token: 0x040022E7 RID: 8935
			public UISkinData.PanelStyle scrollbarBackgroundStyle;

			// Token: 0x040022E8 RID: 8936
			public ColorBlock scrollbarHandleColors;

			// Token: 0x040022E9 RID: 8937
			public Sprite scrollbarHandleImage;
		}
	}
}
