using System;
using UnityEngine;

namespace EntityStates.Jellyfish
{
	// Token: 0x0200080C RID: 2060
	public class SwimState : BaseState
	{
		// Token: 0x06002ECF RID: 11983 RVA: 0x000C734D File Offset: 0x000C554D
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
		}

		// Token: 0x06002ED0 RID: 11984 RVA: 0x000C7364 File Offset: 0x000C5564
		public override void Update()
		{
			base.Update();
			if (base.inputBank)
			{
				this.skill1InputReceived = base.inputBank.skill1.down;
				this.skill2InputReceived |= base.inputBank.skill2.down;
				this.skill3InputReceived |= base.inputBank.skill3.down;
				this.skill4InputReceived |= base.inputBank.skill4.down;
				this.jumpInputReceived |= base.inputBank.jump.down;
			}
		}

		// Token: 0x06002ED1 RID: 11985 RVA: 0x000C7414 File Offset: 0x000C5614
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				if (base.inputBank)
				{
					if (base.rigidbodyMotor)
					{
						base.rigidbodyMotor.moveVector = base.inputBank.moveVector * base.characterBody.moveSpeed;
						if (this.modelAnimator)
						{
							this.modelAnimator.SetFloat("swim.rate", Vector3.Magnitude(base.rigidbodyMotor.rigid.velocity));
						}
					}
					if (base.rigidbodyDirection)
					{
						base.rigidbodyDirection.aimDirection = base.GetAimRay().direction;
					}
				}
				if (base.skillLocator)
				{
					if (base.skillLocator.primary && this.skill1InputReceived)
					{
						base.skillLocator.primary.ExecuteIfReady();
					}
					if (base.skillLocator.secondary && this.skill2InputReceived)
					{
						base.skillLocator.secondary.ExecuteIfReady();
					}
					if (base.skillLocator.utility && this.skill3InputReceived)
					{
						base.skillLocator.utility.ExecuteIfReady();
					}
					if (base.skillLocator.special && this.skill4InputReceived)
					{
						base.skillLocator.special.ExecuteIfReady();
					}
				}
			}
		}

		// Token: 0x04002C0D RID: 11277
		private Animator modelAnimator;

		// Token: 0x04002C0E RID: 11278
		private bool skill1InputReceived;

		// Token: 0x04002C0F RID: 11279
		private bool skill2InputReceived;

		// Token: 0x04002C10 RID: 11280
		private bool skill3InputReceived;

		// Token: 0x04002C11 RID: 11281
		private bool skill4InputReceived;

		// Token: 0x04002C12 RID: 11282
		private bool jumpInputReceived;
	}
}
