using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x0200064D RID: 1613
	public class TooltipProvider : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000320 RID: 800
		// (get) Token: 0x0600240F RID: 9231 RVA: 0x000A9590 File Offset: 0x000A7790
		private bool tooltipIsAvailable
		{
			get
			{
				return this.titleColor != Color.clear;
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06002410 RID: 9232 RVA: 0x000A95A2 File Offset: 0x000A77A2
		public string titleText
		{
			get
			{
				if (!string.IsNullOrEmpty(this.overrideTitleText))
				{
					return this.overrideTitleText;
				}
				if (this.titleToken == null)
				{
					return null;
				}
				return Language.GetString(this.titleToken);
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06002411 RID: 9233 RVA: 0x000A95CD File Offset: 0x000A77CD
		public string bodyText
		{
			get
			{
				if (!string.IsNullOrEmpty(this.overrideBodyText))
				{
					return this.overrideBodyText;
				}
				if (this.bodyToken == null)
				{
					return null;
				}
				return Language.GetString(this.bodyToken);
			}
		}

		// Token: 0x06002412 RID: 9234 RVA: 0x000A95F8 File Offset: 0x000A77F8
		public void SetContent(TooltipContent tooltipContent)
		{
			this.titleToken = tooltipContent.titleToken;
			this.overrideTitleText = tooltipContent.overrideTitleText;
			this.titleColor = tooltipContent.titleColor;
			this.bodyToken = tooltipContent.bodyToken;
			this.overrideBodyText = tooltipContent.overrideBodyText;
			this.bodyColor = tooltipContent.bodyColor;
			this.disableTitleRichText = tooltipContent.disableTitleRichText;
			this.disableBodyRichText = tooltipContent.disableBodyRichText;
		}

		// Token: 0x06002413 RID: 9235 RVA: 0x000A9665 File Offset: 0x000A7865
		private void OnDisable()
		{
			TooltipController.RemoveTooltip(this);
		}

		// Token: 0x06002414 RID: 9236 RVA: 0x000A9670 File Offset: 0x000A7870
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			MPEventSystem mpeventSystem = EventSystem.current as MPEventSystem;
			if (mpeventSystem != null && this.tooltipIsAvailable)
			{
				TooltipController.SetTooltip(mpeventSystem, this, eventData.position);
			}
		}

		// Token: 0x06002415 RID: 9237 RVA: 0x000A96A8 File Offset: 0x000A78A8
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			MPEventSystem mpeventSystem = EventSystem.current as MPEventSystem;
			if (mpeventSystem != null && this.tooltipIsAvailable)
			{
				TooltipController.RemoveTooltip(mpeventSystem, this);
			}
		}

		// Token: 0x06002416 RID: 9238 RVA: 0x000A96D8 File Offset: 0x000A78D8
		public static TooltipContent GetPlayerNameTooltipContent(string userName)
		{
			string stringFormatted = Language.GetStringFormatted("PLAYER_NAME_TOOLTIP_FORMAT", new object[]
			{
				userName
			});
			return new TooltipContent
			{
				overrideTitleText = stringFormatted,
				disableTitleRichText = true,
				titleColor = TooltipProvider.playerColor
			};
		}

		// Token: 0x040026FE RID: 9982
		public string titleToken = "";

		// Token: 0x040026FF RID: 9983
		public Color titleColor = Color.clear;

		// Token: 0x04002700 RID: 9984
		public string bodyToken = "";

		// Token: 0x04002701 RID: 9985
		public Color bodyColor;

		// Token: 0x04002702 RID: 9986
		public string overrideTitleText = "";

		// Token: 0x04002703 RID: 9987
		public string overrideBodyText = "";

		// Token: 0x04002704 RID: 9988
		public bool disableTitleRichText;

		// Token: 0x04002705 RID: 9989
		public bool disableBodyRichText;

		// Token: 0x04002706 RID: 9990
		[NonSerialized]
		public int userCount;

		// Token: 0x04002707 RID: 9991
		private static readonly Color playerColor = new Color32(242, 65, 65, byte.MaxValue);
	}
}
