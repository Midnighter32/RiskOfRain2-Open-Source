using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Paladin.PaladinWeapon
{
	// Token: 0x020000FC RID: 252
	internal class BarrierUp : BaseState
	{
		// Token: 0x060004DB RID: 1243 RVA: 0x00014A24 File Offset: 0x00012C24
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(BarrierUp.soundEffectString, base.gameObject);
			this.stopwatch = 0f;
			this.paladinBarrierController = base.GetComponent<PaladinBarrierController>();
			if (this.paladinBarrierController)
			{
				this.paladinBarrierController.EnableBarrier();
			}
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x00014A77 File Offset: 0x00012C77
		public override void OnExit()
		{
			base.OnExit();
			if (this.paladinBarrierController)
			{
				this.paladinBarrierController.DisableBarrier();
			}
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x00014A98 File Offset: 0x00012C98
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= 0.1f && !base.inputBank.skill2.down && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040004BC RID: 1212
		public static float duration = 5f;

		// Token: 0x040004BD RID: 1213
		public static string soundEffectString;

		// Token: 0x040004BE RID: 1214
		private float stopwatch;

		// Token: 0x040004BF RID: 1215
		private PaladinBarrierController paladinBarrierController;
	}
}
