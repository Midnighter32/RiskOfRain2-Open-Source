using System;
using RoR2;
using UnityEngine;

namespace EntityStates.QuestVolatileBattery
{
	// Token: 0x020007A6 RID: 1958
	public class QuestVolatileBatteryBaseState : BaseState
	{
		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06002CC1 RID: 11457 RVA: 0x000BCC87 File Offset: 0x000BAE87
		// (set) Token: 0x06002CC2 RID: 11458 RVA: 0x000BCC8F File Offset: 0x000BAE8F
		private protected NetworkedBodyAttachment networkedBodyAttachment { protected get; private set; }

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06002CC3 RID: 11459 RVA: 0x000BCC98 File Offset: 0x000BAE98
		// (set) Token: 0x06002CC4 RID: 11460 RVA: 0x000BCCA0 File Offset: 0x000BAEA0
		private protected HealthComponent attachedHealthComponent { protected get; private set; }

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06002CC5 RID: 11461 RVA: 0x000BCCA9 File Offset: 0x000BAEA9
		// (set) Token: 0x06002CC6 RID: 11462 RVA: 0x000BCCB1 File Offset: 0x000BAEB1
		private protected CharacterModel attachedCharacterModel { protected get; private set; }

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06002CC7 RID: 11463 RVA: 0x000BCCBA File Offset: 0x000BAEBA
		// (set) Token: 0x06002CC8 RID: 11464 RVA: 0x000BCCC2 File Offset: 0x000BAEC2
		private protected Transform[] displays { protected get; private set; } = Array.Empty<Transform>();

		// Token: 0x06002CC9 RID: 11465 RVA: 0x000BCCCC File Offset: 0x000BAECC
		public override void OnEnter()
		{
			base.OnEnter();
			this.networkedBodyAttachment = base.GetComponent<NetworkedBodyAttachment>();
			if (this.networkedBodyAttachment && this.networkedBodyAttachment.attachedBody)
			{
				this.attachedHealthComponent = this.networkedBodyAttachment.attachedBody.healthComponent;
				ModelLocator modelLocator = this.networkedBodyAttachment.attachedBody.modelLocator;
				if (modelLocator)
				{
					Transform modelTransform = modelLocator.modelTransform;
					if (modelTransform)
					{
						this.attachedCharacterModel = modelTransform.GetComponent<CharacterModel>();
					}
				}
			}
		}
	}
}
