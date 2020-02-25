using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005BB RID: 1467
	[RequireComponent(typeof(RectTransform))]
	public class ExpBar : MonoBehaviour
	{
		// Token: 0x060022CA RID: 8906 RVA: 0x000970FC File Offset: 0x000952FC
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x060022CB RID: 8907 RVA: 0x0009710C File Offset: 0x0009530C
		public void Update()
		{
			TeamIndex teamIndex = this.source ? this.source.teamIndex : TeamIndex.Neutral;
			float x = 0f;
			if (this.source && TeamManager.instance)
			{
				x = Mathf.InverseLerp(TeamManager.instance.GetTeamCurrentLevelExperience(teamIndex), TeamManager.instance.GetTeamNextLevelExperience(teamIndex), TeamManager.instance.GetTeamExperience(teamIndex));
			}
			if (this.fillRectTransform)
			{
				Rect rect = this.rectTransform.rect;
				Rect rect2 = this.fillRectTransform.rect;
				this.fillRectTransform.anchorMin = new Vector2(0f, 0f);
				this.fillRectTransform.anchorMax = new Vector2(x, 1f);
				this.fillRectTransform.sizeDelta = new Vector2(1f, 1f);
			}
		}

		// Token: 0x04002062 RID: 8290
		public CharacterMaster source;

		// Token: 0x04002063 RID: 8291
		public RectTransform fillRectTransform;

		// Token: 0x04002064 RID: 8292
		private RectTransform rectTransform;
	}
}
