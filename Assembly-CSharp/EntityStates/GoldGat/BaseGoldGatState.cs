using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GoldGat
{
	// Token: 0x02000861 RID: 2145
	public class BaseGoldGatState : EntityState
	{
		// Token: 0x06003069 RID: 12393 RVA: 0x000D0838 File Offset: 0x000CEA38
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
					this.bodyMaster = this.body.master;
					this.bodyInputBank = this.bodyGameObject.GetComponent<InputBankTest>();
					this.bodyEquipmentSlot = this.body.equipmentSlot;
					ModelLocator component = this.body.GetComponent<ModelLocator>();
					if (component)
					{
						this.bodyAimAnimator = component.modelTransform.GetComponent<AimAnimator>();
					}
					this.LinkToDisplay();
				}
			}
		}

		// Token: 0x0600306A RID: 12394 RVA: 0x000D08F4 File Offset: 0x000CEAF4
		private void LinkToDisplay()
		{
			if (this.linkedToDisplay)
			{
				return;
			}
			if (this.bodyEquipmentSlot)
			{
				this.gunTransform = this.bodyEquipmentSlot.FindActiveEquipmentDisplay();
				if (this.gunTransform)
				{
					this.gunChildLocator = this.gunTransform.GetComponentInChildren<ChildLocator>();
					if (this.gunChildLocator && base.modelLocator)
					{
						base.modelLocator.modelTransform = this.gunChildLocator.transform;
						this.gunAnimator = base.GetModelAnimator();
						this.linkedToDisplay = true;
					}
				}
			}
		}

		// Token: 0x0600306B RID: 12395 RVA: 0x000D098C File Offset: 0x000CEB8C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && this.bodyInputBank)
			{
				if (this.bodyInputBank.activateEquipment.justPressed)
				{
					this.shouldFire = !this.shouldFire;
				}
				if (this.body.inventory.GetItemCount(ItemIndex.AutoCastEquipment) > 0)
				{
					this.shouldFire = true;
				}
			}
			this.LinkToDisplay();
			if (this.bodyAimAnimator && this.gunAnimator)
			{
				this.bodyAimAnimator.UpdateAnimatorParameters(this.gunAnimator, -45f, 45f, 0f, 0f);
			}
		}

		// Token: 0x0600306C RID: 12396 RVA: 0x000D0A38 File Offset: 0x000CEC38
		protected bool CheckReturnToIdle()
		{
			if (!base.isAuthority)
			{
				return false;
			}
			if ((this.bodyMaster && this.bodyMaster.money <= 0U) || !this.shouldFire)
			{
				this.outer.SetNextState(new GoldGatIdle
				{
					shouldFire = this.shouldFire
				});
				return true;
			}
			return false;
		}

		// Token: 0x04002EA2 RID: 11938
		protected NetworkedBodyAttachment networkedBodyAttachment;

		// Token: 0x04002EA3 RID: 11939
		protected GameObject bodyGameObject;

		// Token: 0x04002EA4 RID: 11940
		protected CharacterBody body;

		// Token: 0x04002EA5 RID: 11941
		protected ChildLocator gunChildLocator;

		// Token: 0x04002EA6 RID: 11942
		protected Animator gunAnimator;

		// Token: 0x04002EA7 RID: 11943
		protected Transform gunTransform;

		// Token: 0x04002EA8 RID: 11944
		protected CharacterMaster bodyMaster;

		// Token: 0x04002EA9 RID: 11945
		protected EquipmentSlot bodyEquipmentSlot;

		// Token: 0x04002EAA RID: 11946
		protected InputBankTest bodyInputBank;

		// Token: 0x04002EAB RID: 11947
		protected AimAnimator bodyAimAnimator;

		// Token: 0x04002EAC RID: 11948
		public bool shouldFire;

		// Token: 0x04002EAD RID: 11949
		private bool linkedToDisplay;
	}
}
