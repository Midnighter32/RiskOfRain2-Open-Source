using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005DC RID: 1500
	[RequireComponent(typeof(RectTransform))]
	public class ExpBar : MonoBehaviour
	{
		// Token: 0x0600219A RID: 8602 RVA: 0x0009E158 File Offset: 0x0009C358
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x0009E168 File Offset: 0x0009C368
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

		// Token: 0x04002453 RID: 9299
		public CharacterMaster source;

		// Token: 0x04002454 RID: 9300
		public RectTransform fillRectTransform;

		// Token: 0x04002455 RID: 9301
		private RectTransform rectTransform;
	}
}
