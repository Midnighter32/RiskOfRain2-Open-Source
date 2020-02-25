using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200064E RID: 1614
	public class VoteInfoPanelController : MonoBehaviour
	{
		// Token: 0x060025FC RID: 9724 RVA: 0x000A5292 File Offset: 0x000A3492
		private void Awake()
		{
			if (RoR2Application.isInSinglePlayer)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060025FD RID: 9725 RVA: 0x000A52A8 File Offset: 0x000A34A8
		private void AllocateIndicators(int desiredIndicatorCount)
		{
			while (this.indicators.Count > desiredIndicatorCount)
			{
				int index = this.indicators.Count - 1;
				UnityEngine.Object.Destroy(this.indicators[index].gameObject);
				this.indicators.RemoveAt(index);
			}
			while (this.indicators.Count < desiredIndicatorCount)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.indicatorPrefab, this.container);
				gameObject.SetActive(true);
				this.indicators.Add(new VoteInfoPanelController.IndicatorInfo
				{
					gameObject = gameObject,
					image = gameObject.GetComponentInChildren<Image>(),
					tooltipProvider = gameObject.GetComponentInChildren<TooltipProvider>()
				});
			}
			this.timerPanelObject.transform.SetAsLastSibling();
		}

		// Token: 0x060025FE RID: 9726 RVA: 0x000A5364 File Offset: 0x000A3564
		public void UpdateElements()
		{
			int num = 0;
			if (this.voteController)
			{
				num = this.voteController.GetVoteCount();
			}
			this.AllocateIndicators(num);
			for (int i = 0; i < num; i++)
			{
				VoteController.UserVote vote = this.voteController.GetVote(i);
				this.indicators[i].image.sprite = (vote.receivedVote ? this.hasVotedSprite : this.hasNotVotedSprite);
				string userName;
				if (vote.networkUserObject)
				{
					NetworkUser component = vote.networkUserObject.GetComponent<NetworkUser>();
					if (component)
					{
						userName = component.GetNetworkPlayerName().GetResolvedName();
					}
					else
					{
						userName = Language.GetString("PLAYER_NAME_UNAVAILABLE");
					}
				}
				else
				{
					userName = Language.GetString("PLAYER_NAME_DISCONNECTED");
				}
				this.indicators[i].tooltipProvider.SetContent(TooltipProvider.GetPlayerNameTooltipContent(userName));
			}
			bool flag = this.voteController && this.voteController.timerStartCondition != VoteController.TimerStartCondition.Never;
			this.timerPanelObject.SetActive(flag);
			if (flag)
			{
				float num2 = this.voteController.timer;
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				int num3 = Mathf.FloorToInt(num2 * 0.016666668f);
				int num4 = (int)num2 - num3 * 60;
				this.timerLabel.text = string.Format("{0}:{1:00}", num3, num4);
			}
		}

		// Token: 0x060025FF RID: 9727 RVA: 0x000A54D8 File Offset: 0x000A36D8
		private void Update()
		{
			this.UpdateElements();
		}

		// Token: 0x040023B8 RID: 9144
		public GameObject indicatorPrefab;

		// Token: 0x040023B9 RID: 9145
		public Sprite hasNotVotedSprite;

		// Token: 0x040023BA RID: 9146
		public Sprite hasVotedSprite;

		// Token: 0x040023BB RID: 9147
		public RectTransform container;

		// Token: 0x040023BC RID: 9148
		public GameObject timerPanelObject;

		// Token: 0x040023BD RID: 9149
		public TextMeshProUGUI timerLabel;

		// Token: 0x040023BE RID: 9150
		public VoteController voteController;

		// Token: 0x040023BF RID: 9151
		private readonly List<VoteInfoPanelController.IndicatorInfo> indicators = new List<VoteInfoPanelController.IndicatorInfo>();

		// Token: 0x0200064F RID: 1615
		private struct IndicatorInfo
		{
			// Token: 0x040023C0 RID: 9152
			public GameObject gameObject;

			// Token: 0x040023C1 RID: 9153
			public Image image;

			// Token: 0x040023C2 RID: 9154
			public TooltipProvider tooltipProvider;
		}
	}
}
