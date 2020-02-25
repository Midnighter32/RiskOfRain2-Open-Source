using System;
using UnityEngine;

namespace EntityStates.Jellyfish
{
	// Token: 0x0200080B RID: 2059
	public class Dash : BaseState
	{
		// Token: 0x06002ECB RID: 11979 RVA: 0x000C7260 File Offset: 0x000C5460
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.moveVector = base.rigidbodyMotor.rigid.transform.forward * base.characterBody.moveSpeed * Dash.speedCoefficient;
			}
		}

		// Token: 0x06002ECC RID: 11980 RVA: 0x000C72C8 File Offset: 0x000C54C8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.rigidbodyMotor && this.modelAnimator)
			{
				this.modelAnimator.SetFloat("swim.rate", Vector3.Magnitude(base.rigidbodyMotor.rigid.velocity));
			}
			if (base.fixedAge >= Dash.duration)
			{
				this.outer.SetNextState(new SwimState());
			}
		}

		// Token: 0x04002C0A RID: 11274
		public static float duration = 1.8f;

		// Token: 0x04002C0B RID: 11275
		public static float speedCoefficient = 2f;

		// Token: 0x04002C0C RID: 11276
		private Animator modelAnimator;
	}
}
