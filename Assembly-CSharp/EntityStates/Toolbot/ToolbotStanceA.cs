using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x0200076F RID: 1903
	public class ToolbotStanceA : ToolbotStanceBase
	{
		// Token: 0x06002BDD RID: 11229 RVA: 0x000B975E File Offset: 0x000B795E
		public override void OnEnter()
		{
			base.OnEnter();
			this.swapStateType = typeof(ToolbotStanceB);
			if (NetworkServer.active)
			{
				base.SetEquipmentSlot(0);
			}
		}

		// Token: 0x06002BDE RID: 11230 RVA: 0x000B9784 File Offset: 0x000B7984
		protected override GenericSkill GetCurrentPrimarySkill()
		{
			return base.GetPrimarySkill1();
		}
	}
}
