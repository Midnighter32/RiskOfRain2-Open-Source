using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000659 RID: 1625
	public class VoteInfoPanelController : MonoBehaviour
	{
		// Token: 0x06002458 RID: 9304 RVA: 0x000AA756 File Offset: 0x000A8956
		private void Awake()
		{
			if (RoR2Application.isInSinglePlayer)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06002459 RID: 9305 RVA: 0x000AA76C File Offset: 0x000A896C
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

		// Token: 0x0600245A RID: 9306 RVA: 0x000AA828 File Offset: 0x000A8A28
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

		// Token: 0x0600245B RID: 9307 RVA: 0x000AA99C File Offset: 0x000A8B9C
		private void Update()
		{
			this.UpdateElements();
		}

		// Token: 0x04002751 RID: 10065
		public GameObject indicatorPrefab;

		// Token: 0x04002752 RID: 10066
		public Sprite hasNotVotedSprite;

		// Token: 0x04002753 RID: 10067
		public Sprite hasVotedSprite;

		// Token: 0x04002754 RID: 10068
		public RectTransform container;

		// Token: 0x04002755 RID: 10069
		public GameObject timerPanelObject;

		// Token: 0x04002756 RID: 10070
		public TextMeshProUGUI timerLabel;

		// Token: 0x04002757 RID: 10071
		public VoteController voteController;

		// Token: 0x04002758 RID: 10072
		private readonly List<VoteInfoPanelController.IndicatorInfo> indicators = new List<VoteInfoPanelController.IndicatorInfo>();

		// Token: 0x0200065A RID: 1626
		private struct IndicatorInfo
		{
			// Token: 0x04002759 RID: 10073
			public GameObject gameObject;

			// Token: 0x0400275A RID: 10074
			public Image image;

			// Token: 0x0400275B RID: 10075
			public TooltipProvider tooltipProvider;
		}
	}
}
