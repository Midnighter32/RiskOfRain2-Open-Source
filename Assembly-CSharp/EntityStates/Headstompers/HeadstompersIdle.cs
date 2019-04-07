using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Headstompers
{
	// Token: 0x0200015E RID: 350
	public class HeadstompersIdle : BaseHeadstompersState
	{
		// Token: 0x060006CC RID: 1740 RVA: 0x0002065C File Offset: 0x0001E85C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				bool flag = base.jumpButtonDown;
				if (JetpackController.FindJetpackController(this.bodyGameObject))
				{
					flag = false;
				}
				this.inputStopwatch = (flag ? (this.inputStopwatch + Time.deltaTime) : 0f);
				if (base.isGrounded)
				{
					this.jumpBoostOk = true;
				}
				else if (this.jumpBoostOk && flag && this.bodyMotor)
				{
					Vector3 velocity = this.bodyMotor.velocity;
					if (velocity.y > 0f)
					{
						velocity.y *= 2f;
						this.bodyMotor.velocity = velocity;
						this.jumpBoostOk = false;
					}
					EffectManager.instance.SimpleImpactEffect(HeadstompersIdle.jumpEffect, this.bodyGameObject.transform.position, Vector3.up, true);
				}
				if (this.inputStopwatch >= HeadstompersIdle.inputConfirmationDelay && !base.isGrounded)
				{
					this.outer.SetNextState(new HeadstompersCharge());
					return;
				}
			}
		}

		// Token: 0x0400085A RID: 2138
		private float inputStopwatch;

		// Token: 0x0400085B RID: 2139
		public static float inputConfirmationDelay = 0.1f;

		// Token: 0x0400085C RID: 2140
		private bool jumpBoostOk;

		// Token: 0x0400085D RID: 2141
		public static GameObject jumpEffect;
	}
}
