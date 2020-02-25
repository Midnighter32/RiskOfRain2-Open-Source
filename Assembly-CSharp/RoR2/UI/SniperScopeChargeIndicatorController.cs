using System;
using EntityStates.Sniper.Scope;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000631 RID: 1585
	[RequireComponent(typeof(HudElement))]
	public class SniperScopeChargeIndicatorController : MonoBehaviour
	{
		// Token: 0x06002566 RID: 9574 RVA: 0x000A2D49 File Offset: 0x000A0F49
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x06002567 RID: 9575 RVA: 0x000A2D58 File Offset: 0x000A0F58
		private void FixedUpdate()
		{
			float fillAmount = 0f;
			if (this.hudElement.targetCharacterBody)
			{
				SkillLocator component = this.hudElement.targetCharacterBody.GetComponent<SkillLocator>();
				if (component && component.secondary)
				{
					EntityStateMachine stateMachine = component.secondary.stateMachine;
					if (stateMachine)
					{
						ScopeSniper scopeSniper = stateMachine.state as ScopeSniper;
						if (scopeSniper != null)
						{
							fillAmount = scopeSniper.charge;
						}
					}
				}
			}
			if (this.image)
			{
				this.image.fillAmount = fillAmount;
			}
		}

		// Token: 0x0400231E RID: 8990
		private GameObject sourceGameObject;

		// Token: 0x0400231F RID: 8991
		private HudElement hudElement;

		// Token: 0x04002320 RID: 8992
		public Image image;
	}
}
