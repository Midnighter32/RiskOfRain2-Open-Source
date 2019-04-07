using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000B8 RID: 184
	internal class HoverState : BaseState
	{
		// Token: 0x060003A4 RID: 932 RVA: 0x0000F098 File Offset: 0x0000D298
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			base.PlayAnimation("Body", "Idle");
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x0000F0BC File Offset: 0x0000D2BC
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

		// Token: 0x060003A6 RID: 934 RVA: 0x0000F134 File Offset: 0x0000D334
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

		// Token: 0x04000364 RID: 868
		private Animator modelAnimator;

		// Token: 0x04000365 RID: 869
		private bool skill1InputReceived;

		// Token: 0x04000366 RID: 870
		private bool skill2InputReceived;

		// Token: 0x04000367 RID: 871
		private bool skill3InputReceived;

		// Token: 0x04000368 RID: 872
		private bool skill4InputReceived;
	}
}
