using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.ImpMonster.Weapon
{
	// Token: 0x02000828 RID: 2088
	public class CloakPermanent : BaseState
	{
		// Token: 0x06002F4D RID: 12109 RVA: 0x000C9F9A File Offset: 0x000C819A
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.Cloak);
			}
		}

		// Token: 0x06002F4E RID: 12110 RVA: 0x000C9FC2 File Offset: 0x000C81C2
		public override void OnExit()
		{
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.RemoveBuff(BuffIndex.Cloak);
			}
			base.OnExit();
		}

		// Token: 0x06002F4F RID: 12111 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
	}
}
