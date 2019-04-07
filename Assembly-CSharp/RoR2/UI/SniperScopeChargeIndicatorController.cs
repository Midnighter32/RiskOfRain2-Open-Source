using System;
using EntityStates.Sniper.Scope;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200063E RID: 1598
	[RequireComponent(typeof(HudElement))]
	public class SniperScopeChargeIndicatorController : MonoBehaviour
	{
		// Token: 0x060023CD RID: 9165 RVA: 0x000A8455 File Offset: 0x000A6655
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x060023CE RID: 9166 RVA: 0x000A8464 File Offset: 0x000A6664
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

		// Token: 0x040026C1 RID: 9921
		private GameObject sourceGameObject;

		// Token: 0x040026C2 RID: 9922
		private HudElement hudElement;

		// Token: 0x040026C3 RID: 9923
		public Image image;
	}
}
