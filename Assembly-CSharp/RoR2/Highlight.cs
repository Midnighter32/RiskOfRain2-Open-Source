using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200030E RID: 782
	public class Highlight : MonoBehaviour
	{
		// Token: 0x17000166 RID: 358
		// (get) Token: 0x0600103F RID: 4159 RVA: 0x0005181B File Offset: 0x0004FA1B
		public static ReadOnlyCollection<Highlight> readonlyHighlightList
		{
			get
			{
				return Highlight._readonlyHighlightList;
			}
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x00051822 File Offset: 0x0004FA22
		private void Awake()
		{
			this.displayNameProvider = base.GetComponent<IDisplayNameProvider>();
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x00051830 File Offset: 0x0004FA30
		public void OnEnable()
		{
			Highlight.highlightList.Add(this);
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x0005183D File Offset: 0x0004FA3D
		public void OnDisable()
		{
			Highlight.highlightList.Remove(this);
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x0005184C File Offset: 0x0004FA4C
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

		// Token: 0x04001430 RID: 5168
		private static List<Highlight> highlightList = new List<Highlight>();

		// Token: 0x04001431 RID: 5169
		private static ReadOnlyCollection<Highlight> _readonlyHighlightList = new ReadOnlyCollection<Highlight>(Highlight.highlightList);

		// Token: 0x04001432 RID: 5170
		private IDisplayNameProvider displayNameProvider;

		// Token: 0x04001433 RID: 5171
		[HideInInspector]
		public PickupIndex pickupIndex;

		// Token: 0x04001434 RID: 5172
		public Renderer targetRenderer;

		// Token: 0x04001435 RID: 5173
		public float strength = 1f;

		// Token: 0x04001436 RID: 5174
		public Highlight.HighlightColor highlightColor;

		// Token: 0x04001437 RID: 5175
		public bool isOn;

		// Token: 0x0200030F RID: 783
		public enum HighlightColor
		{
			// Token: 0x04001439 RID: 5177
			interactive,
			// Token: 0x0400143A RID: 5178
			teleporter,
			// Token: 0x0400143B RID: 5179
			pickup,
			// Token: 0x0400143C RID: 5180
			unavailable
		}
	}
}
