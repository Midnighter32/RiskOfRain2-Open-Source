using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000712 RID: 1810
	public class HoverState : BaseState
	{
		// Token: 0x06002A42 RID: 10818 RVA: 0x000B1E45 File Offset: 0x000B0045
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			base.PlayAnimation("Body", "Idle");
		}

		// Token: 0x06002A43 RID: 10819 RVA: 0x000B1E6C File Offset: 0x000B006C
		public override void Update()
		{
			base.Update();
			if (base.inputBank)
			{
				this.skill1InputReceived = base.inputBank.skill1.down;
				this.skill2InputReceived = base.inputBank.skill2.down;
				this.skill3InputReceived = base.inputBank.skill3.down;
				this.skill4InputReceived = base.inputBank.skill4.down;
			}
		}

		// Token: 0x06002A44 RID: 10820 RVA: 0x000B1EE4 File Offset: 0x000B00E4
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
							this.modelAnimator.SetFloat("fly.rate", Vector3.Magnitude(base.rigidbodyMotor.rigid.velocity));
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

		// Token: 0x0400261E RID: 9758
		private Animator modelAnimator;

		// Token: 0x0400261F RID: 9759
		private bool skill1InputReceived;

		// Token: 0x04002620 RID: 9760
		private bool skill2InputReceived;

		// Token: 0x04002621 RID: 9761
		private bool skill3InputReceived;

		// Token: 0x04002622 RID: 9762
		private bool skill4InputReceived;
	}
}
