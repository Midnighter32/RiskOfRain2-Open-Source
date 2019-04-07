using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000BE RID: 190
	internal class BaseMelee : BaseState
	{
		// Token: 0x060003B9 RID: 953 RVA: 0x0000F534 File Offset: 0x0000D734
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

		// Token: 0x060003BA RID: 954 RVA: 0x0000F5AC File Offset: 0x0000D7AC
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

		// Token: 0x04000370 RID: 880
		protected RootMotionAccumulator rootMotionAccumulator;
	}
}
