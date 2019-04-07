using System;
using UnityEngine;

namespace EntityStates.Jellyfish
{
	// Token: 0x02000135 RID: 309
	internal class Dash : BaseState
	{
		// Token: 0x060005F4 RID: 1524 RVA: 0x0001B5B0 File Offset: 0x000197B0
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.moveVector = base.rigidbodyMotor.rigid.transform.forward * base.characterBody.moveSpeed * Dash.speedCoefficient;
			}
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0001B618 File Offset: 0x00019818
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

		// Token: 0x040006E6 RID: 1766
		public static float duration = 1.8f;

		// Token: 0x040006E7 RID: 1767
		public static float speedCoefficient = 2f;

		// Token: 0x040006E8 RID: 1768
		private Animator modelAnimator;
	}
}
