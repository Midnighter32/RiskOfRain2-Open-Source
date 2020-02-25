using System;
using UnityEngine;

namespace EntityStates.ScavMonster
{
	// Token: 0x0200078D RID: 1933
	public class EnergyCannonState : BaseState
	{
		// Token: 0x06002C5D RID: 11357 RVA: 0x000BB3E2 File Offset: 0x000B95E2
		public override void OnEnter()
		{
			base.OnEnter();
			this.muzzleTransform = base.FindModelChild(EnergyCannonState.muzzleName);
		}

		// Token: 0x06002C5E RID: 11358 RVA: 0x000B23CF File Offset: 0x000B05CF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		// Token: 0x06002C5F RID: 11359 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002C60 RID: 11360 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002861 RID: 10337
		public static string muzzleName;

		// Token: 0x04002862 RID: 10338
		protected Transform muzzleTransform;
	}
}
