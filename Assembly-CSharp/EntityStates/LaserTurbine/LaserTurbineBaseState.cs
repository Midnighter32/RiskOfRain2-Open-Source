using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LaserTurbine
{
	// Token: 0x020007F8 RID: 2040
	public class LaserTurbineBaseState : EntityState
	{
		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06002E5D RID: 11869 RVA: 0x000C5472 File Offset: 0x000C3672
		// (set) Token: 0x06002E5E RID: 11870 RVA: 0x000C547A File Offset: 0x000C367A
		private protected LaserTurbineController laserTurbineController { protected get; private set; }

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06002E5F RID: 11871 RVA: 0x000C5483 File Offset: 0x000C3683
		// (set) Token: 0x06002E60 RID: 11872 RVA: 0x000C548B File Offset: 0x000C368B
		private protected SimpleRotateToDirection simpleRotateToDirection { protected get; private set; }

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06002E61 RID: 11873 RVA: 0x000C5494 File Offset: 0x000C3694
		protected CharacterBody ownerBody
		{
			get
			{
				GenericOwnership genericOwnership = this.genericOwnership;
				return this.bodyGetComponent.Get((genericOwnership != null) ? genericOwnership.ownerObject : null);
			}
		}

		// Token: 0x06002E62 RID: 11874 RVA: 0x000C54B3 File Offset: 0x000C36B3
		public override void OnEnter()
		{
			base.OnEnter();
			this.genericOwnership = base.GetComponent<GenericOwnership>();
			this.simpleLeash = base.GetComponent<SimpleLeash>();
			this.simpleRotateToDirection = base.GetComponent<SimpleRotateToDirection>();
			this.laserTurbineController = base.GetComponent<LaserTurbineController>();
		}

		// Token: 0x06002E63 RID: 11875 RVA: 0x000AA35A File Offset: 0x000A855A
		public virtual float GetChargeFraction()
		{
			return 0f;
		}

		// Token: 0x06002E64 RID: 11876 RVA: 0x000C54EB File Offset: 0x000C36EB
		protected InputBankTest GetInputBank()
		{
			CharacterBody ownerBody = this.ownerBody;
			if (ownerBody == null)
			{
				return null;
			}
			return ownerBody.inputBank;
		}

		// Token: 0x06002E65 RID: 11877 RVA: 0x000C54FE File Offset: 0x000C36FE
		protected Ray GetAimRay()
		{
			return new Ray(base.transform.position, base.transform.forward);
		}

		// Token: 0x06002E66 RID: 11878 RVA: 0x000C551B File Offset: 0x000C371B
		protected Transform GetMuzzleTransform()
		{
			return base.transform;
		}

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06002E67 RID: 11879 RVA: 0x0000B933 File Offset: 0x00009B33
		protected virtual bool shouldFollow
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002E68 RID: 11880 RVA: 0x000C5524 File Offset: 0x000C3724
		public override void Update()
		{
			base.Update();
			if (this.ownerBody && this.shouldFollow)
			{
				this.simpleLeash.leashOrigin = this.ownerBody.corePosition;
				this.simpleRotateToDirection.targetRotation = Quaternion.LookRotation(this.ownerBody.inputBank.aimDirection);
			}
		}

		// Token: 0x06002E69 RID: 11881 RVA: 0x000C5584 File Offset: 0x000C3784
		protected float GetDamage()
		{
			float num = 1f;
			if (this.ownerBody)
			{
				num = this.ownerBody.damage;
				if (this.ownerBody.inventory)
				{
					num *= (float)this.ownerBody.inventory.GetItemCount(ItemIndex.LaserTurbine);
				}
			}
			return num;
		}

		// Token: 0x04002B86 RID: 11142
		private GenericOwnership genericOwnership;

		// Token: 0x04002B87 RID: 11143
		private SimpleLeash simpleLeash;

		// Token: 0x04002B89 RID: 11145
		private MemoizedGetComponent<CharacterBody> bodyGetComponent;
	}
}
