using System;
using RoR2;

namespace EntityStates
{
	// Token: 0x020006F8 RID: 1784
	public class BaseBodyAttachmentState : EntityState
	{
		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x0600296C RID: 10604 RVA: 0x000AE5E3 File Offset: 0x000AC7E3
		// (set) Token: 0x0600296D RID: 10605 RVA: 0x000AE5EB File Offset: 0x000AC7EB
		private protected NetworkedBodyAttachment bodyAttachment { protected get; private set; }

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x0600296E RID: 10606 RVA: 0x000AE5F4 File Offset: 0x000AC7F4
		// (set) Token: 0x0600296F RID: 10607 RVA: 0x000AE5FC File Offset: 0x000AC7FC
		private protected CharacterBody attachedBody { protected get; private set; }

		// Token: 0x06002970 RID: 10608 RVA: 0x000AE605 File Offset: 0x000AC805
		public override void OnEnter()
		{
			base.OnEnter();
			this.bodyAttachment = base.GetComponent<NetworkedBodyAttachment>();
			this.attachedBody = (this.bodyAttachment ? this.bodyAttachment.attachedBody : null);
		}
	}
}
