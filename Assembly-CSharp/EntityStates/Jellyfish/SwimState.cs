using System;
using UnityEngine;

namespace EntityStates.Jellyfish
{
	// Token: 0x02000136 RID: 310
	internal class SwimState : BaseState
	{
		// Token: 0x060005F8 RID: 1528 RVA: 0x0001B69D File Offset: 0x0001989D
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0001B6B4 File Offset: 0x000198B4
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

		// Token: 0x060005FA RID: 1530 RVA: 0x0001B764 File Offset: 0x00019964
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

		// Token: 0x040006E9 RID: 1769
		private Animator modelAnimator;

		// Token: 0x040006EA RID: 1770
		private bool skill1InputReceived;

		// Token: 0x040006EB RID: 1771
		private bool skill2InputReceived;

		// Token: 0x040006EC RID: 1772
		private bool skill3InputReceived;

		// Token: 0x040006ED RID: 1773
		private bool skill4InputReceived;

		// Token: 0x040006EE RID: 1774
		private bool jumpInputReceived;
	}
}
