using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace EntityStates.Headstompers
{
	// Token: 0x0200083F RID: 2111
	public class BaseHeadstompersState : EntityState
	{
		// Token: 0x06002FC8 RID: 12232 RVA: 0x000CCBF8 File Offset: 0x000CADF8
		public static BaseHeadstompersState FindForBody(CharacterBody body)
		{
			for (int i = 0; i < BaseHeadstompersState.instancesList.Count; i++)
			{
				if (BaseHeadstompersState.instancesList[i].body == body)
				{
					return BaseHeadstompersState.instancesList[i];
				}
			}
			return null;
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06002FC9 RID: 12233 RVA: 0x000CCC3A File Offset: 0x000CAE3A
		protected bool jumpButtonDown
		{
			get
			{
				return this.bodyInputBank && this.bodyInputBank.jump.down;
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06002FCA RID: 12234 RVA: 0x000CCC5B File Offset: 0x000CAE5B
		protected bool slamButtonDown
		{
			get
			{
				return this.bodyInputBank && this.bodyInputBank.interact.down;
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06002FCB RID: 12235 RVA: 0x000CCC7C File Offset: 0x000CAE7C
		protected bool isGrounded
		{
			get
			{
				return this.bodyMotor && this.bodyMotor.isGrounded;
			}
		}

		// Token: 0x06002FCC RID: 12236 RVA: 0x000CCC98 File Offset: 0x000CAE98
		public override void OnEnter()
		{
			base.OnEnter();
			BaseHeadstompersState.instancesList.Add(this);
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

		// Token: 0x06002FCD RID: 12237 RVA: 0x000CCD20 File Offset: 0x000CAF20
		public override void OnExit()
		{
			BaseHeadstompersState.instancesList.Remove(this);
			base.OnExit();
		}

		// Token: 0x06002FCE RID: 12238 RVA: 0x000CCD34 File Offset: 0x000CAF34
		protected bool ReturnToIdleIfGrounded()
		{
			if (this.bodyMotor && this.bodyMotor.isGrounded)
			{
				this.outer.SetNextState(new HeadstompersIdle());
				return true;
			}
			return false;
		}

		// Token: 0x04002D9A RID: 11674
		private static readonly List<BaseHeadstompersState> instancesList = new List<BaseHeadstompersState>();

		// Token: 0x04002D9B RID: 11675
		protected NetworkedBodyAttachment networkedBodyAttachment;

		// Token: 0x04002D9C RID: 11676
		protected GameObject bodyGameObject;

		// Token: 0x04002D9D RID: 11677
		protected CharacterBody body;

		// Token: 0x04002D9E RID: 11678
		protected CharacterMotor bodyMotor;

		// Token: 0x04002D9F RID: 11679
		protected InputBankTest bodyInputBank;
	}
}
