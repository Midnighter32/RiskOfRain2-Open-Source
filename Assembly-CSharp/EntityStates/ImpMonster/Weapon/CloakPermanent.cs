using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.ImpMonster.Weapon
{
	// Token: 0x0200014C RID: 332
	public class CloakPermanent : BaseState
	{
		// Token: 0x06000660 RID: 1632 RVA: 0x0001DC17 File Offset: 0x0001BE17
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.Cloak);
			}
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0001DC3F File Offset: 0x0001BE3F
		public override void OnExit()
		{
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.RemoveBuff(BuffIndex.Cloak);
			}
			base.OnExit();
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
	}
}
