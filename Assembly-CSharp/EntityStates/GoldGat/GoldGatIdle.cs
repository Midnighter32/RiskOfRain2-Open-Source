using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GoldGat
{
	// Token: 0x02000862 RID: 2146
	public class GoldGatIdle : BaseGoldGatState
	{
		// Token: 0x0600306E RID: 12398 RVA: 0x000D0A91 File Offset: 0x000CEC91
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(GoldGatIdle.windDownSoundString, base.gameObject);
		}

		// Token: 0x0600306F RID: 12399 RVA: 0x000D0AAC File Offset: 0x000CECAC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.gunAnimator.SetFloat("Crank.playbackRate", 0f, 1f, Time.fixedDeltaTime);
			if (base.isAuthority && this.shouldFire && this.bodyMaster.money > 0U && this.bodyEquipmentSlot.stock > 0)
			{
				this.outer.SetNextState(new GoldGatFire
				{
					shouldFire = this.shouldFire
				});
				return;
			}
		}

		// Token: 0x04002EAE RID: 11950
		public static string windDownSoundString;
	}
}
