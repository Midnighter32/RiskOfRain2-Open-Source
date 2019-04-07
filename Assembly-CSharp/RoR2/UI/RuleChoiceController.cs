using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200062F RID: 1583
	public class RuleChoiceController : MonoBehaviour
	{
		// Token: 0x06002384 RID: 9092 RVA: 0x000A7334 File Offset: 0x000A5534
		private void OnEnable()
		{
			RuleChoiceController.instancesList.Add(this);
		}

		// Token: 0x06002385 RID: 9093 RVA: 0x000A7341 File Offset: 0x000A5541
		private void OnDisable()
		{
			RuleChoiceController.instancesList.Remove(this);
		}

		// Token: 0x06002386 RID: 9094 RVA: 0x000A734F File Offset: 0x000A554F
		static RuleChoiceController()
		{
			PreGameRuleVoteController.onVotesUpdated += delegate()
			{
				foreach (RuleChoiceController ruleChoiceController in RuleChoiceController.instancesList)
				{
					ruleChoiceController.UpdateFromVotes();
				}
			};
		}

		// Token: 0x06002387 RID: 9095 RVA: 0x000A7370 File Offset: 0x000A5570
		private void Start()
		{
			this.UpdateFromVotes();
		}

		// Token: 0x06002388 RID: 9096 RVA: 0x000A7378 File Offset: 0x000A5578
		public void UpdateFromVotes()
		{
			int num = PreGameRuleVoteController.votesForEachChoice[this.currentChoiceDef.globalIndex];
			bool isInSinglePlayer = RoR2Application.isInSinglePlayer;
			if (num > 0 && !isInSinglePlayer)
			{
				this.voteCounter.enabled = true;
				this.voteCounter.text = num.ToString();
			}
			else
			{
				this.voteCounter.enabled = false;
			}
			bool enabled = false;
			NetworkUser networkUser = this.FindNetworkUser();
			if (networkUser)
			{
				PreGameRuleVoteController preGameRuleVoteController = PreGameRuleVoteController.FindForUser(networkUser);
				if (preGameRuleVoteController)
				{
					enabled = preGameRuleVoteController.IsChoiceVoted(this.currentChoiceDef);
				}
			}
			this.selectionDisplayPanel.enabled = enabled;
		}

		// Token: 0x06002389 RID: 9097 RVA: 0x000A740C File Offset: 0x000A560C
		public void SetChoice([NotNull] RuleChoiceDef newChoiceDef)
		{
			if (newChoiceDef == this.currentChoiceDef)
			{
				return;
			}
			this.currentChoiceDef = newChoiceDef;
			base.gameObject.name = "Choice (" + this.currentChoiceDef.globalName + ")";
			this.image.material = ((this.currentChoiceDef.materialPath == null) ? null : Resources.Load<Material>(this.currentChoiceDef.materialPath));
			this.image.sprite = Resources.Load<Sprite>(this.currentChoiceDef.spritePath);
			this.tooltipProvider.titleToken = this.currentChoiceDef.tooltipNameToken;
			this.tooltipProvider.titleColor = this.currentChoiceDef.tooltipNameColor;
			this.tooltipProvider.bodyToken = this.currentChoiceDef.tooltipBodyToken;
			this.tooltipProvider.bodyColor = this.currentChoiceDef.tooltipBodyColor;
			this.UpdateFromVotes();
		}

		// Token: 0x0600238A RID: 9098 RVA: 0x000A74F3 File Offset: 0x000A56F3
		private NetworkUser FindNetworkUser()
		{
			return ((MPEventSystem)EventSystem.current).localUser.currentNetworkUser;
		}

		// Token: 0x0600238B RID: 9099 RVA: 0x000A750C File Offset: 0x000A570C
		public void OnClick()
		{
			if (!this.canVote)
			{
				return;
			}
			NetworkUser networkUser = this.FindNetworkUser();
			Debug.Log(networkUser);
			if (networkUser)
			{
				PreGameRuleVoteController preGameRuleVoteController = PreGameRuleVoteController.FindForUser(networkUser);
				if (preGameRuleVoteController)
				{
					int choiceValue = this.currentChoiceDef.localIndex;
					if (preGameRuleVoteController.IsChoiceVoted(this.currentChoiceDef))
					{
						choiceValue = -1;
					}
					preGameRuleVoteController.SetVote(this.currentChoiceDef.ruleDef.globalIndex, choiceValue);
					Debug.LogFormat("voteController.SetVote({0}, {1})", new object[]
					{
						this.currentChoiceDef.ruleDef.globalIndex,
						this.currentChoiceDef.localIndex
					});
					return;
				}
				Debug.Log("voteController=null");
			}
		}

		// Token: 0x0400267E RID: 9854
		private static readonly List<RuleChoiceController> instancesList = new List<RuleChoiceController>();

		// Token: 0x0400267F RID: 9855
		[HideInInspector]
		public RuleBookViewerStrip strip;

		// Token: 0x04002680 RID: 9856
		public Image image;

		// Token: 0x04002681 RID: 9857
		public TooltipProvider tooltipProvider;

		// Token: 0x04002682 RID: 9858
		public TextMeshProUGUI voteCounter;

		// Token: 0x04002683 RID: 9859
		public Image selectionDisplayPanel;

		// Token: 0x04002684 RID: 9860
		public bool canVote;

		// Token: 0x04002685 RID: 9861
		private RuleChoiceDef currentChoiceDef;
	}
}
