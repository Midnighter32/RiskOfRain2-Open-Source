using System;
using RoR2;

namespace EntityStates.VagrantNovaItem
{
	// Token: 0x02000741 RID: 1857
	public class ReadyState : BaseVagrantNovaItemState
	{
		// Token: 0x06002B14 RID: 11028 RVA: 0x000B559D File Offset: 0x000B379D
		public override void OnEnter()
		{
			base.OnEnter();
			CharacterBody attachedBody = base.attachedBody;
			this.attachedHealthComponent = ((attachedBody != null) ? attachedBody.healthComponent : null);
			base.SetChargeSparkEmissionRateMultiplier(1f);
		}

		// Token: 0x06002B15 RID: 11029 RVA: 0x000B55C8 File Offset: 0x000B37C8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && this.attachedHealthComponent.combinedHealthFraction <= ReadyState.healthFractionThreshold)
			{
				this.outer.SetNextState(new ChargeState());
			}
		}

		// Token: 0x040026ED RID: 9965
		public static float healthFractionThreshold = 0.2f;

		// Token: 0x040026EE RID: 9966
		private HealthComponent attachedHealthComponent;
	}
}
