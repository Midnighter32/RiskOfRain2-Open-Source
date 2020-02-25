using System;

namespace EntityStates.HermitCrab
{
	// Token: 0x0200083A RID: 2106
	public class Burrowed : BaseState
	{
		// Token: 0x06002FAF RID: 12207 RVA: 0x000CC621 File Offset: 0x000CA821
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Body", "Burrowed", 0.1f);
		}

		// Token: 0x06002FB0 RID: 12208 RVA: 0x000CC640 File Offset: 0x000CA840
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

		// Token: 0x06002FB1 RID: 12209 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002D7A RID: 11642
		public static float mortarCooldown;

		// Token: 0x04002D7B RID: 11643
		public float duration;
	}
}
