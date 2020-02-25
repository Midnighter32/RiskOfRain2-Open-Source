using System;
using UnityEngine;

namespace EntityStates.ScavMonster
{
	// Token: 0x02000796 RID: 1942
	public class SackBaseState : BaseState
	{
		// Token: 0x06002C84 RID: 11396 RVA: 0x000BBC9D File Offset: 0x000B9E9D
		public override void OnEnter()
		{
			base.OnEnter();
			this.muzzleTransform = base.FindModelChild(SackBaseState.muzzleName);
		}

		// Token: 0x06002C85 RID: 11397 RVA: 0x000B23CF File Offset: 0x000B05CF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		// Token: 0x06002C86 RID: 11398 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002C87 RID: 11399 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002893 RID: 10387
		public static string muzzleName;

		// Token: 0x04002894 RID: 10388
		protected Transform muzzleTransform;
	}
}
