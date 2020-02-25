using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200022E RID: 558
	public class Highlight : MonoBehaviour
	{
		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000C81 RID: 3201 RVA: 0x000385D5 File Offset: 0x000367D5
		public static ReadOnlyCollection<Highlight> readonlyHighlightList
		{
			get
			{
				return Highlight._readonlyHighlightList;
			}
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x000385DC File Offset: 0x000367DC
		private void Awake()
		{
			this.displayNameProvider = base.GetComponent<IDisplayNameProvider>();
		}

		// Token: 0x06000C83 RID: 3203 RVA: 0x000385EA File Offset: 0x000367EA
		public void OnEnable()
		{
			Highlight.highlightList.Add(this);
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x000385F7 File Offset: 0x000367F7
		public void OnDisable()
		{
			Highlight.highlightList.Remove(this);
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x00038608 File Offset: 0x00036808
		public Color GetColor()
		{
			switch (this.highlightColor)
			{
			case Highlight.HighlightColor.interactive:
				return ColorCatalog.GetColor(ColorCatalog.ColorIndex.Interactable);
			case Highlight.HighlightColor.teleporter:
				return ColorCatalog.GetColor(ColorCatalog.ColorIndex.Teleporter);
			case Highlight.HighlightColor.pickup:
				return this.pickupIndex.GetPickupColor();
			default:
				return Color.magenta;
			}
		}

		// Token: 0x04000C6E RID: 3182
		private static List<Highlight> highlightList = new List<Highlight>();

		// Token: 0x04000C6F RID: 3183
		private static ReadOnlyCollection<Highlight> _readonlyHighlightList = new ReadOnlyCollection<Highlight>(Highlight.highlightList);

		// Token: 0x04000C70 RID: 3184
		private IDisplayNameProvider displayNameProvider;

		// Token: 0x04000C71 RID: 3185
		[HideInInspector]
		public PickupIndex pickupIndex;

		// Token: 0x04000C72 RID: 3186
		public Renderer targetRenderer;

		// Token: 0x04000C73 RID: 3187
		public float strength = 1f;

		// Token: 0x04000C74 RID: 3188
		public Highlight.HighlightColor highlightColor;

		// Token: 0x04000C75 RID: 3189
		public bool isOn;

		// Token: 0x0200022F RID: 559
		public enum HighlightColor
		{
			// Token: 0x04000C77 RID: 3191
			interactive,
			// Token: 0x04000C78 RID: 3192
			teleporter,
			// Token: 0x04000C79 RID: 3193
			pickup,
			// Token: 0x04000C7A RID: 3194
			unavailable
		}
	}
}
