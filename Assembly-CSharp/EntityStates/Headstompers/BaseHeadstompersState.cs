using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Headstompers
{
	// Token: 0x0200015D RID: 349
	public class BaseHeadstompersState : EntityState
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060006C7 RID: 1735 RVA: 0x00020573 File Offset: 0x0001E773
		protected bool jumpButtonDown
		{
			get
			{
				return this.bodyInputBank && this.bodyInputBank.jump.down;
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x00020594 File Offset: 0x0001E794
		protected bool isGrounded
		{
			get
			{
				return this.bodyMotor && this.bodyMotor.isGrounded;
			}
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x000205B0 File Offset: 0x0001E7B0
		public override void OnEnter()
		{
			base.OnEnter();
			this.networkedBodyAttachment = base.GetComponent<NetworkedBodyAttachment>();
			if (this.networkedBodyAttachment)
			{
				this.bodyGameObject = this.networkedBodyAttachment.attachedBodyObject;
				this.body = this.networkedBodyAttachment.attachedBody;
				if (this.bodyGameObject)
				{
					this.bodyMotor = this.bodyGameObject.GetComponent<CharacterMotor>();
					this.bodyInputBank = this.bodyGameObject.GetComponent<InputBankTest>();
				}
			}
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x0002062D File Offset: 0x0001E82D
		protected bool ReturnToIdleIfGrounded()
		{
			if (this.bodyMotor && this.bodyMotor.isGrounded)
			{
				this.outer.SetNextState(new HeadstompersIdle());
				return true;
			}
			return false;
		}

		// Token: 0x04000855 RID: 2133
		protected NetworkedBodyAttachment networkedBodyAttachment;

		// Token: 0x04000856 RID: 2134
		protected GameObject bodyGameObject;

		// Token: 0x04000857 RID: 2135
		protected CharacterBody body;

		// Token: 0x04000858 RID: 2136
		protected CharacterMotor bodyMotor;

		// Token: 0x04000859 RID: 2137
		protected InputBankTest bodyInputBank;
	}
}
