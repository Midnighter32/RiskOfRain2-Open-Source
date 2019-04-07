using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GoldGat
{
	// Token: 0x0200017C RID: 380
	public class GoldGatIdle : BaseGoldGatState
	{
		// Token: 0x06000751 RID: 1873 RVA: 0x00023C15 File Offset: 0x00021E15
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(GoldGatIdle.windDownSoundString, base.gameObject);
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00023C30 File Offset: 0x00021E30
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.gunAnimator.SetFloat("Crank.playbackRate", 0f, 1f, Time.fixedDeltaTime);
			if (base.isAuthority && this.shouldFire && this.bodyMaster.money > 0u && this.bodyEquipmentSlot.stock > 0)
			{
				this.outer.SetNextState(new GoldGatFire
				{
					shouldFire = this.shouldFire
				});
				return;
			}
		}

		// Token: 0x04000940 RID: 2368
		public static string windDownSoundString;
	}
}
