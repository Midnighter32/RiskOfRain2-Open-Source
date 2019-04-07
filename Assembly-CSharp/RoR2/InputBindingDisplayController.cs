using System;
using Rewired;
using RoR2.UI;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000406 RID: 1030
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputBindingDisplayController : MonoBehaviour
	{
		// Token: 0x060016EC RID: 5868 RVA: 0x0006D350 File Offset: 0x0006B550
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.guiLabel = base.GetComponent<TextMeshProUGUI>();
			this.label = base.GetComponent<TextMeshPro>();
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x0006D378 File Offset: 0x0006B578
		private void Refresh()
		{
			string glyphString;
			if (this.useExplicitInputSource)
			{
				glyphString = Glyphs.GetGlyphString(this.eventSystemLocator.eventSystem, this.actionName, this.axisRange, this.explicitInputSource);
			}
			else
			{
				glyphString = Glyphs.GetGlyphString(this.eventSystemLocator.eventSystem, this.actionName, AxisRange.Full);
			}
			if (this.guiLabel)
			{
				this.guiLabel.text = glyphString;
				return;
			}
			if (this.label)
			{
				this.label.text = glyphString;
			}
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x0006D3FD File Offset: 0x0006B5FD
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x04001A20 RID: 6688
		public string actionName;

		// Token: 0x04001A21 RID: 6689
		public AxisRange axisRange;

		// Token: 0x04001A22 RID: 6690
		public bool useExplicitInputSource;

		// Token: 0x04001A23 RID: 6691
		public MPEventSystem.InputSource explicitInputSource;

		// Token: 0x04001A24 RID: 6692
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04001A25 RID: 6693
		private TextMeshProUGUI guiLabel;

		// Token: 0x04001A26 RID: 6694
		private TextMeshPro label;
	}
}
