using System;
using UnityEngine.Networking;

namespace EntityStates.QuestVolatileBattery
{
	// Token: 0x020007A7 RID: 1959
	public class Monitor : QuestVolatileBatteryBaseState
	{
		// Token: 0x06002CCB RID: 11467 RVA: 0x000BCD67 File Offset: 0x000BAF67
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x06002CCC RID: 11468 RVA: 0x000BCD7C File Offset: 0x000BAF7C
		private void FixedUpdateServer()
		{
			if (!base.attachedHealthComponent)
			{
				return;
			}
			float combinedHealthFraction = base.attachedHealthComponent.combinedHealthFraction;
			if (combinedHealthFraction <= Monitor.healthFractionDetonationThreshold && Monitor.healthFractionDetonationThreshold < this.previousHealthFraction)
			{
				this.outer.SetNextState(new CountDown());
			}
			this.previousHealthFraction = combinedHealthFraction;
		}

		// Token: 0x040028E6 RID: 10470
		private float previousHealthFraction;

		// Token: 0x040028E7 RID: 10471
		private static readonly float healthFractionDetonationThreshold = 0.5f;
	}
}
