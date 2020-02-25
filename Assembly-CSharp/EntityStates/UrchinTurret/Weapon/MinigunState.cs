using System;
using RoR2;
using UnityEngine;

namespace EntityStates.UrchinTurret.Weapon
{
	// Token: 0x02000905 RID: 2309
	public class MinigunState : BaseState
	{
		// Token: 0x06003387 RID: 13191 RVA: 0x000DFC0A File Offset: 0x000DDE0A
		public override void OnEnter()
		{
			base.OnEnter();
			this.muzzleTransform = base.FindModelChild(MinigunState.muzzleName);
		}

		// Token: 0x06003388 RID: 13192 RVA: 0x000DA121 File Offset: 0x000D8321
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.StartAimMode(2f, false);
		}

		// Token: 0x06003389 RID: 13193 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x0600338A RID: 13194 RVA: 0x000DA161 File Offset: 0x000D8361
		protected ref InputBankTest.ButtonState skillButtonState
		{
			get
			{
				return ref base.inputBank.skill1;
			}
		}

		// Token: 0x0600338B RID: 13195 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04003307 RID: 13063
		public static string muzzleName;

		// Token: 0x04003308 RID: 13064
		protected Transform muzzleTransform;
	}
}
