using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000642 RID: 1602
	public class TooltipProvider : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x170003DF RID: 991
		// (get) Token: 0x060025AE RID: 9646 RVA: 0x000A4058 File Offset: 0x000A2258
		private bool tooltipIsAvailable
		{
			get
			{
				return this.titleColor != Color.clear;
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x060025AF RID: 9647 RVA: 0x000A406A File Offset: 0x000A226A
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

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x060025B0 RID: 9648 RVA: 0x000A4095 File Offset: 0x000A2295
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

		// Token: 0x060025B1 RID: 9649 RVA: 0x000A40C0 File Offset: 0x000A22C0
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

		// Token: 0x060025B2 RID: 9650 RVA: 0x000A412D File Offset: 0x000A232D
		private void OnDisable()
		{
			TooltipController.RemoveTooltip(this);
		}

		// Token: 0x060025B3 RID: 9651 RVA: 0x000A4138 File Offset: 0x000A2338
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			MPEventSystem mpeventSystem = EventSystem.current as MPEventSystem;
			if (mpeventSystem != null && this.tooltipIsAvailable)
			{
				TooltipController.SetTooltip(mpeventSystem, this, eventData.position);
			}
		}

		// Token: 0x060025B4 RID: 9652 RVA: 0x000A4170 File Offset: 0x000A2370
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			MPEventSystem mpeventSystem = EventSystem.current as MPEventSystem;
			if (mpeventSystem != null && this.tooltipIsAvailable)
			{
				TooltipController.RemoveTooltip(mpeventSystem, this);
			}
		}

		// Token: 0x060025B5 RID: 9653 RVA: 0x000A41A0 File Offset: 0x000A23A0
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

		// Token: 0x04002363 RID: 9059
		public string titleToken = "";

		// Token: 0x04002364 RID: 9060
		public Color titleColor = Color.clear;

		// Token: 0x04002365 RID: 9061
		public string bodyToken = "";

		// Token: 0x04002366 RID: 9062
		public Color bodyColor;

		// Token: 0x04002367 RID: 9063
		public string overrideTitleText = "";

		// Token: 0x04002368 RID: 9064
		public string overrideBodyText = "";

		// Token: 0x04002369 RID: 9065
		public bool disableTitleRichText;

		// Token: 0x0400236A RID: 9066
		public bool disableBodyRichText;

		// Token: 0x0400236B RID: 9067
		[NonSerialized]
		public int userCount;

		// Token: 0x0400236C RID: 9068
		private static readonly Color playerColor = new Color32(242, 65, 65, byte.MaxValue);
	}
}
