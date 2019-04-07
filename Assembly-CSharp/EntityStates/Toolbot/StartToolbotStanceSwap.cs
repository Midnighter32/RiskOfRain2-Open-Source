using System;
using System.Linq;
using RoR2;

namespace EntityStates.Toolbot
{
	// Token: 0x020000EA RID: 234
	public class StartToolbotStanceSwap : BaseState
	{
		// Token: 0x06000485 RID: 1157 RVA: 0x00012EE8 File Offset: 0x000110E8
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.hasAuthority)
			{
				this.stanceStateMachine = base.gameObject.GetComponents<EntityStateMachine>().FirstOrDefault((EntityStateMachine c) => c.customName == "Stance");
				EntityStateMachine entityStateMachine = this.stanceStateMachine;
				ToolbotStanceBase toolbotStanceBase = ((entityStateMachine != null) ? entityStateMachine.state : null) as ToolbotStanceBase;
				if (toolbotStanceBase != null && toolbotStanceBase.swapStateType != null)
				{
					this.stanceStateMachine.SetNextState(new ToolbotStanceSwap
					{
						nextStanceState = toolbotStanceBase.swapStateType
					});
				}
			}
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x00012F7D File Offset: 0x0001117D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.outer.SetNextStateToMain();
		}

		// Token: 0x04000452 RID: 1106
		private EntityStateMachine stanceStateMachine;
	}
}
