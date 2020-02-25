using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ClayBruiser.Weapon
{
	// Token: 0x020008C8 RID: 2248
	public class MinigunState : BaseState
	{
		// Token: 0x06003268 RID: 12904 RVA: 0x000DA0E4 File Offset: 0x000D82E4
		public override void OnEnter()
		{
			base.OnEnter();
			this.muzzleTransform = base.FindModelChild(MinigunState.muzzleName);
			if (NetworkServer.active && base.characterBody)
			{
				base.characterBody.AddBuff(MinigunState.slowBuff);
			}
		}

		// Token: 0x06003269 RID: 12905 RVA: 0x000DA121 File Offset: 0x000D8321
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.StartAimMode(2f, false);
		}

		// Token: 0x0600326A RID: 12906 RVA: 0x000DA135 File Offset: 0x000D8335
		public override void OnExit()
		{
			if (NetworkServer.active && base.characterBody)
			{
				base.characterBody.RemoveBuff(MinigunState.slowBuff);
			}
			base.OnExit();
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x0600326B RID: 12907 RVA: 0x000DA161 File Offset: 0x000D8361
		protected ref InputBankTest.ButtonState skillButtonState
		{
			get
			{
				return ref base.inputBank.skill1;
			}
		}

		// Token: 0x0600326C RID: 12908 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04003161 RID: 12641
		public static string muzzleName;

		// Token: 0x04003162 RID: 12642
		private static readonly BuffIndex slowBuff;

		// Token: 0x04003163 RID: 12643
		protected Transform muzzleTransform;
	}
}
