using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200063C RID: 1596
	public class SkillIcon : MonoBehaviour
	{
		// Token: 0x060023C7 RID: 9159 RVA: 0x000A7FFC File Offset: 0x000A61FC
		private void Update()
		{
			if (this.targetSkill)
			{
				if (this.tooltipProvider)
				{
					this.tooltipProvider.titleToken = this.targetSkill.skillNameToken;
					this.tooltipProvider.bodyToken = this.targetSkill.skillDescriptionToken;
				}
				float cooldownRemaining = this.targetSkill.cooldownRemaining;
				float baseRechargeInterval = this.targetSkill.baseRechargeInterval;
				int stock = this.targetSkill.stock;
				bool flag = stock > 0 || cooldownRemaining == 0f;
				bool flag2 = this.targetSkill.CanExecute();
				if (this.previousStock < stock)
				{
					Util.PlaySound("Play_UI_cooldownRefresh", RoR2Application.instance.gameObject);
				}
				if (this.animator)
				{
					if (this.targetSkill.maxStock > 1)
					{
						this.animator.SetBool(this.animatorStackString, true);
					}
					else
					{
						this.animator.SetBool(this.animatorStackString, false);
					}
				}
				if (this.isReadyPanelObject)
				{
					this.isReadyPanelObject.SetActive(flag2);
				}
				if (!this.wasReady && flag && this.flashPanelObject)
				{
					this.flashPanelObject.SetActive(true);
				}
				if (this.cooldownText)
				{
					if (flag)
					{
						this.cooldownText.gameObject.SetActive(false);
					}
					else
					{
						SkillIcon.sharedStringBuilder.Clear();
						SkillIcon.sharedStringBuilder.AppendInt(Mathf.CeilToInt(cooldownRemaining), 0u, uint.MaxValue);
						this.cooldownText.SetText(SkillIcon.sharedStringBuilder);
						this.cooldownText.gameObject.SetActive(true);
					}
				}
				if (this.iconImage)
				{
					this.iconImage.enabled = true;
					this.iconImage.color = (flag2 ? Color.white : Color.gray);
					this.iconImage.sprite = this.targetSkill.icon;
				}
				if (this.cooldownRemapPanel)
				{
					float num = (baseRechargeInterval >= Mathf.Epsilon) ? (1f - cooldownRemaining / baseRechargeInterval) : 1f;
					this.cooldownRemapPanel.enabled = (num < 1f);
					this.cooldownRemapPanel.color = new Color(1f, 1f, 1f, 1f - cooldownRemaining / baseRechargeInterval);
				}
				if (this.stockText)
				{
					if (this.targetSkill.maxStock > 1)
					{
						this.stockText.gameObject.SetActive(true);
						SkillIcon.sharedStringBuilder.Clear();
						SkillIcon.sharedStringBuilder.AppendInt(this.targetSkill.stock, 0u, uint.MaxValue);
						this.stockText.SetText(SkillIcon.sharedStringBuilder);
					}
					else
					{
						this.stockText.gameObject.SetActive(false);
					}
				}
				this.wasReady = flag;
				this.previousStock = stock;
				return;
			}
			if (this.tooltipProvider)
			{
				this.tooltipProvider.titleToken = "";
				this.tooltipProvider.bodyToken = "";
			}
			if (this.cooldownText)
			{
				this.cooldownText.gameObject.SetActive(false);
			}
			if (this.stockText)
			{
				this.stockText.gameObject.SetActive(false);
			}
			if (this.iconImage)
			{
				this.iconImage.enabled = false;
				this.iconImage.sprite = null;
			}
		}

		// Token: 0x040026B0 RID: 9904
		public SkillSlot targetSkillSlot;

		// Token: 0x040026B1 RID: 9905
		public PlayerCharacterMasterController playerCharacterMasterController;

		// Token: 0x040026B2 RID: 9906
		public GenericSkill targetSkill;

		// Token: 0x040026B3 RID: 9907
		public Image iconImage;

		// Token: 0x040026B4 RID: 9908
		public RawImage cooldownRemapPanel;

		// Token: 0x040026B5 RID: 9909
		public TextMeshProUGUI cooldownText;

		// Token: 0x040026B6 RID: 9910
		public TextMeshProUGUI stockText;

		// Token: 0x040026B7 RID: 9911
		public GameObject flashPanelObject;

		// Token: 0x040026B8 RID: 9912
		public GameObject isReadyPanelObject;

		// Token: 0x040026B9 RID: 9913
		public Animator animator;

		// Token: 0x040026BA RID: 9914
		public string animatorStackString;

		// Token: 0x040026BB RID: 9915
		public bool wasReady;

		// Token: 0x040026BC RID: 9916
		public int previousStock;

		// Token: 0x040026BD RID: 9917
		public TooltipProvider tooltipProvider;

		// Token: 0x040026BE RID: 9918
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
