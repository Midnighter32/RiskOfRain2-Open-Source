using System;
using Rewired;
using RoR2.UI;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035F RID: 863
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputBindingDisplayController : MonoBehaviour
	{
		// Token: 0x060014F2 RID: 5362 RVA: 0x00059700 File Offset: 0x00057900
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.guiLabel = base.GetComponent<TextMeshProUGUI>();
			this.label = base.GetComponent<TextMeshPro>();
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x00059728 File Offset: 0x00057928
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

		// Token: 0x060014F4 RID: 5364 RVA: 0x000597AD File Offset: 0x000579AD
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x04001390 RID: 5008
		public string actionName;

		// Token: 0x04001391 RID: 5009
		public AxisRange axisRange;

		// Token: 0x04001392 RID: 5010
		public bool useExplicitInputSource;

		// Token: 0x04001393 RID: 5011
		public MPEventSystem.InputSource explicitInputSource;

		// Token: 0x04001394 RID: 5012
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04001395 RID: 5013
		private TextMeshProUGUI guiLabel;

		// Token: 0x04001396 RID: 5014
		private TextMeshPro label;
	}
}
