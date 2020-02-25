using System;
using System.Linq;
using RoR2;

namespace EntityStates.Toolbot
{
	// Token: 0x02000772 RID: 1906
	public class StartToolbotStanceSwap : BaseState
	{
		// Token: 0x06002BE8 RID: 11240 RVA: 0x000B9960 File Offset: 0x000B7B60
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				this.stanceStateMachine = base.gameObject.GetComponents<EntityStateMachine>().FirstOrDefault((EntityStateMachine c) => c.customName == "Stance");
				EntityStateMachine entityStateMachine = this.stanceStateMachine;
				ToolbotStanceBase toolbotStanceBase = ((entityStateMachine != null) ? entityStateMachine.state : null) as ToolbotStanceBase;
				if (toolbotStanceBase != null && toolbotStanceBase.swapStateType != null)
				{
					this.stanceStateMachine.SetNextState(new ToolbotStanceSwap
					{
						previousStanceState = toolbotStanceBase.GetType(),
						nextStanceState = toolbotStanceBase.swapStateType
					});
				}
			}
		}

		// Token: 0x06002BE9 RID: 11241 RVA: 0x000B9A04 File Offset: 0x000B7C04
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.outer.SetNextStateToMain();
		}

		// Token: 0x04002809 RID: 10249
		private EntityStateMachine stanceStateMachine;
	}
}
