using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200062F RID: 1583
	public class SkillIcon : MonoBehaviour
	{
		// Token: 0x06002560 RID: 9568 RVA: 0x000A2874 File Offset: 0x000A0A74
		private void Update()
		{
			if (this.targetSkill)
			{
				if (this.tooltipProvider)
				{
					Color color = Color.gray;
					SurvivorIndex survivorIndexFromBodyIndex = SurvivorCatalog.GetSurvivorIndexFromBodyIndex(this.targetSkill.characterBody.bodyIndex);
					if (survivorIndexFromBodyIndex != SurvivorIndex.None)
					{
						color = SurvivorCatalog.GetSurvivorDef(survivorIndexFromBodyIndex).primaryColor;
						float h;
						float s;
						float num;
						Color.RGBToHSV(color, out h, out s, out num);
						num = ((num > 0.7f) ? 0.7f : num);
						color = Color.HSVToRGB(h, s, num);
					}
					this.tooltipProvider.titleColor = color;
					this.tooltipProvider.titleToken = this.targetSkill.skillNameToken;
					this.tooltipProvider.bodyToken = this.targetSkill.skillDescriptionToken;
				}
				float cooldownRemaining = this.targetSkill.cooldownRemaining;
				float num2 = this.targetSkill.CalculateFinalRechargeInterval();
				int stock = this.targetSkill.stock;
				bool flag = stock > 0 || cooldownRemaining == 0f;
				bool flag2 = this.targetSkill.IsReady();
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
						SkillIcon.sharedStringBuilder.AppendInt(Mathf.CeilToInt(cooldownRemaining), 0U, uint.MaxValue);
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
					float num3 = 1f;
					if (num2 >= Mathf.Epsilon)
					{
						num3 = 1f - cooldownRemaining / num2;
					}
					float num4 = num3;
					this.cooldownRemapPanel.enabled = (num4 < 1f);
					this.cooldownRemapPanel.color = new Color(1f, 1f, 1f, num3);
				}
				if (this.stockText)
				{
					if (this.targetSkill.maxStock > 1)
					{
						this.stockText.gameObject.SetActive(true);
						SkillIcon.sharedStringBuilder.Clear();
						SkillIcon.sharedStringBuilder.AppendInt(this.targetSkill.stock, 0U, uint.MaxValue);
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
				this.tooltipProvider.bodyColor = Color.gray;
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

		// Token: 0x0400230D RID: 8973
		public SkillSlot targetSkillSlot;

		// Token: 0x0400230E RID: 8974
		public PlayerCharacterMasterController playerCharacterMasterController;

		// Token: 0x0400230F RID: 8975
		public GenericSkill targetSkill;

		// Token: 0x04002310 RID: 8976
		public Image iconImage;

		// Token: 0x04002311 RID: 8977
		public RawImage cooldownRemapPanel;

		// Token: 0x04002312 RID: 8978
		public TextMeshProUGUI cooldownText;

		// Token: 0x04002313 RID: 8979
		public TextMeshProUGUI stockText;

		// Token: 0x04002314 RID: 8980
		public GameObject flashPanelObject;

		// Token: 0x04002315 RID: 8981
		public GameObject isReadyPanelObject;

		// Token: 0x04002316 RID: 8982
		public Animator animator;

		// Token: 0x04002317 RID: 8983
		public string animatorStackString;

		// Token: 0x04002318 RID: 8984
		public bool wasReady;

		// Token: 0x04002319 RID: 8985
		public int previousStock;

		// Token: 0x0400231A RID: 8986
		public TooltipProvider tooltipProvider;

		// Token: 0x0400231B RID: 8987
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
