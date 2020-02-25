using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200061D RID: 1565
	public class RuleChoiceController : MonoBehaviour
	{
		// Token: 0x06002501 RID: 9473 RVA: 0x000A16C4 File Offset: 0x0009F8C4
		private void OnEnable()
		{
			RuleChoiceController.instancesList.Add(this);
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x000A16D1 File Offset: 0x0009F8D1
		private void OnDisable()
		{
			RuleChoiceController.instancesList.Remove(this);
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x000A16DF File Offset: 0x0009F8DF
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

		// Token: 0x06002504 RID: 9476 RVA: 0x000A1700 File Offset: 0x0009F900
		private void Start()
		{
			this.UpdateFromVotes();
		}

		// Token: 0x06002505 RID: 9477 RVA: 0x000A1708 File Offset: 0x0009F908
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

		// Token: 0x06002506 RID: 9478 RVA: 0x000A179C File Offset: 0x0009F99C
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

		// Token: 0x06002507 RID: 9479 RVA: 0x000A1883 File Offset: 0x0009FA83
		private NetworkUser FindNetworkUser()
		{
			LocalUser localUser = ((MPEventSystem)EventSystem.current).localUser;
			if (localUser == null)
			{
				return null;
			}
			return localUser.currentNetworkUser;
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x000A18A0 File Offset: 0x0009FAA0
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

		// Token: 0x040022C5 RID: 8901
		private static readonly List<RuleChoiceController> instancesList = new List<RuleChoiceController>();

		// Token: 0x040022C6 RID: 8902
		[HideInInspector]
		public RuleBookViewerStrip strip;

		// Token: 0x040022C7 RID: 8903
		public Image image;

		// Token: 0x040022C8 RID: 8904
		public TooltipProvider tooltipProvider;

		// Token: 0x040022C9 RID: 8905
		public TextMeshProUGUI voteCounter;

		// Token: 0x040022CA RID: 8906
		public Image selectionDisplayPanel;

		// Token: 0x040022CB RID: 8907
		public bool canVote;

		// Token: 0x040022CC RID: 8908
		private RuleChoiceDef currentChoiceDef;
	}
}
