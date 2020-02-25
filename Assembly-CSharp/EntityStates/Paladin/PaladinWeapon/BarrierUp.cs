using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Paladin.PaladinWeapon
{
	// Token: 0x020007AD RID: 1965
	public class BarrierUp : BaseState
	{
		// Token: 0x06002CEF RID: 11503 RVA: 0x000BDC60 File Offset: 0x000BBE60
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

		// Token: 0x06002CF0 RID: 11504 RVA: 0x000BDCB3 File Offset: 0x000BBEB3
		public override void OnExit()
		{
			base.OnExit();
			if (this.paladinBarrierController)
			{
				this.paladinBarrierController.DisableBarrier();
			}
		}

		// Token: 0x06002CF1 RID: 11505 RVA: 0x000BDCD4 File Offset: 0x000BBED4
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

		// Token: 0x06002CF2 RID: 11506 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002925 RID: 10533
		public static float duration = 5f;

		// Token: 0x04002926 RID: 10534
		public static string soundEffectString;

		// Token: 0x04002927 RID: 10535
		private float stopwatch;

		// Token: 0x04002928 RID: 10536
		private PaladinBarrierController paladinBarrierController;
	}
}
