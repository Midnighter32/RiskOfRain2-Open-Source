using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GoldGat
{
	// Token: 0x0200017B RID: 379
	public class BaseGoldGatState : EntityState
	{
		// Token: 0x0600074C RID: 1868 RVA: 0x000239BC File Offset: 0x00021BBC
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

		// Token: 0x0600074D RID: 1869 RVA: 0x00023A78 File Offset: 0x00021C78
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

		// Token: 0x0600074E RID: 1870 RVA: 0x00023B10 File Offset: 0x00021D10
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

		// Token: 0x0600074F RID: 1871 RVA: 0x00023BBC File Offset: 0x00021DBC
		protected bool CheckReturnToIdle()
		{
			if (!base.isAuthority)
			{
				return false;
			}
			if ((this.bodyMaster && this.bodyMaster.money <= 0u) || !this.shouldFire)
			{
				this.outer.SetNextState(new GoldGatIdle
				{
					shouldFire = this.shouldFire
				});
				return true;
			}
			return false;
		}

		// Token: 0x04000934 RID: 2356
		protected NetworkedBodyAttachment networkedBodyAttachment;

		// Token: 0x04000935 RID: 2357
		protected GameObject bodyGameObject;

		// Token: 0x04000936 RID: 2358
		protected CharacterBody body;

		// Token: 0x04000937 RID: 2359
		protected ChildLocator gunChildLocator;

		// Token: 0x04000938 RID: 2360
		protected Animator gunAnimator;

		// Token: 0x04000939 RID: 2361
		protected Transform gunTransform;

		// Token: 0x0400093A RID: 2362
		protected CharacterMaster bodyMaster;

		// Token: 0x0400093B RID: 2363
		protected EquipmentSlot bodyEquipmentSlot;

		// Token: 0x0400093C RID: 2364
		protected InputBankTest bodyInputBank;

		// Token: 0x0400093D RID: 2365
		protected AimAnimator bodyAimAnimator;

		// Token: 0x0400093E RID: 2366
		public bool shouldFire;

		// Token: 0x0400093F RID: 2367
		private bool linkedToDisplay;
	}
}
