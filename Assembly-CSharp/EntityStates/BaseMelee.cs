using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000718 RID: 1816
	public class BaseMelee : BaseState
	{
		// Token: 0x06002A57 RID: 10839 RVA: 0x000B22D0 File Offset: 0x000B04D0
		public RootMotionAccumulator InitMeleeRootMotion()
		{
			this.rootMotionAccumulator = base.GetModelRootMotionAccumulator();
			if (this.rootMotionAccumulator)
			{
				this.rootMotionAccumulator.ExtractRootMotion();
			}
			if (base.characterDirection)
			{
				base.characterDirection.forward = base.inputBank.aimDirection;
			}
			if (base.characterMotor)
			{
				base.characterMotor.moveDirection = Vector3.zero;
			}
			return this.rootMotionAccumulator;
		}

		// Token: 0x06002A58 RID: 10840 RVA: 0x000B2348 File Offset: 0x000B0548
		public void UpdateMeleeRootMotion(float scale)
		{
			if (this.rootMotionAccumulator)
			{
				Vector3 a = this.rootMotionAccumulator.ExtractRootMotion();
				if (base.characterMotor)
				{
					base.characterMotor.rootMotion = a * scale;
				}
			}
		}

		// Token: 0x0400262A RID: 9770
		protected RootMotionAccumulator rootMotionAccumulator;
	}
}
