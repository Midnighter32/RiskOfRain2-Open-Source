using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x02000770 RID: 1904
	public class ToolbotStanceB : ToolbotStanceBase
	{
		// Token: 0x06002BE0 RID: 11232 RVA: 0x000B9794 File Offset: 0x000B7994
		public override void OnEnter()
		{
			base.OnEnter();
			this.swapStateType = typeof(ToolbotStanceA);
			if (NetworkServer.active)
			{
				base.SetEquipmentSlot(1);
			}
		}

		// Token: 0x06002BE1 RID: 11233 RVA: 0x000B97BA File Offset: 0x000B79BA
		protected override GenericSkill GetCurrentPrimarySkill()
		{
			return base.GetPrimarySkill2();
		}
	}
}
