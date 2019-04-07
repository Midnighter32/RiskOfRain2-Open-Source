using System;

namespace EntityStates.HermitCrab
{
	// Token: 0x02000158 RID: 344
	internal class Burrowed : BaseState
	{
		// Token: 0x060006AE RID: 1710 RVA: 0x0001FF8D File Offset: 0x0001E18D
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Body", "Burrowed", 0.1f);
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0001FFAC File Offset: 0x0001E1AC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				if (base.inputBank.moveVector.sqrMagnitude > 0.1f)
				{
					this.outer.SetNextState(new BurrowOut());
				}
				if (base.fixedAge >= this.duration && base.inputBank.skill1.down)
				{
					this.outer.SetNextState(new FireMortar());
				}
			}
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000834 RID: 2100
		public static float mortarCooldown;

		// Token: 0x04000835 RID: 2101
		public float duration;
	}
}
