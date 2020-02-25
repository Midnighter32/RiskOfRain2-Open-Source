using System;
using System.Collections.Generic;
using EntityStates.Engi.MineDeployer;

namespace RoR2.Skills
{
	// Token: 0x020004B3 RID: 1203
	public class EngiMineDeployerSkill : SkillDef
	{
		// Token: 0x06001D12 RID: 7442 RVA: 0x0007C6E4 File Offset: 0x0007A8E4
		public override bool CanExecute(GenericSkill skillSlot)
		{
			List<BaseMineDeployerState> instancesList = BaseMineDeployerState.instancesList;
			for (int i = 0; i < instancesList.Count; i++)
			{
				if (instancesList[i].owner == skillSlot.gameObject)
				{
					return false;
				}
			}
			return base.CanExecute(skillSlot);
		}
	}
}
