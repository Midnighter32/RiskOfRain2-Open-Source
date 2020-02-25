using System;
using UnityEngine;

namespace EntityStates.VagrantNovaItem
{
	// Token: 0x02000740 RID: 1856
	public class RechargeState : BaseVagrantNovaItemState
	{
		// Token: 0x06002B11 RID: 11025 RVA: 0x000B5524 File Offset: 0x000B3724
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				int itemStack = base.GetItemStack();
				float num = RechargeState.baseDuration / (float)(itemStack + 1);
				float num2 = base.fixedAge / num;
				if (num2 >= 1f)
				{
					num2 = 1f;
					this.outer.SetNextState(new ReadyState());
				}
				base.SetChargeSparkEmissionRateMultiplier(RechargeState.particleEmissionCurve.Evaluate(num2));
			}
		}

		// Token: 0x040026EA RID: 9962
		public static float baseDuration = 30f;

		// Token: 0x040026EB RID: 9963
		public static AnimationCurve particleEmissionCurve;

		// Token: 0x040026EC RID: 9964
		private float rechargeDuration;
	}
}
