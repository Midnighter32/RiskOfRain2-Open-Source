using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Headstompers
{
	// Token: 0x02000840 RID: 2112
	public class HeadstompersIdle : BaseHeadstompersState
	{
		// Token: 0x06002FD1 RID: 12241 RVA: 0x000CCD70 File Offset: 0x000CAF70
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.inputStopwatch = (base.slamButtonDown ? (this.inputStopwatch + Time.fixedDeltaTime) : 0f);
				if (base.isGrounded)
				{
					this.jumpBoostOk = true;
				}
				else if (this.jumpBoostOk && base.jumpButtonDown && this.bodyMotor)
				{
					Vector3 velocity = this.bodyMotor.velocity;
					if (velocity.y > 0f)
					{
						velocity.y *= 2f;
						this.bodyMotor.velocity = velocity;
						this.jumpBoostOk = false;
					}
					EffectManager.SimpleImpactEffect(HeadstompersIdle.jumpEffect, this.bodyGameObject.transform.position, Vector3.up, true);
				}
				if (this.inputStopwatch >= HeadstompersIdle.inputConfirmationDelay && !base.isGrounded)
				{
					this.outer.SetNextState(new HeadstompersCharge());
					return;
				}
			}
		}

		// Token: 0x04002DA0 RID: 11680
		private float inputStopwatch;

		// Token: 0x04002DA1 RID: 11681
		public static float inputConfirmationDelay = 0.1f;

		// Token: 0x04002DA2 RID: 11682
		private bool jumpBoostOk;

		// Token: 0x04002DA3 RID: 11683
		public static GameObject jumpEffect;
	}
}
