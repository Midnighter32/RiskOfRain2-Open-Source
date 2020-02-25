using System;
using EntityStates.Engi.Mine;
using RoR2.Projectile;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F2 RID: 498
	public class EngiMineAnimator : MonoBehaviour
	{
		// Token: 0x06000A66 RID: 2662 RVA: 0x0002DA8C File Offset: 0x0002BC8C
		private void Start()
		{
			ProjectileGhostController component = base.GetComponent<ProjectileGhostController>();
			if (component)
			{
				this.projectileTransform = component.authorityTransform;
				this.armingStateMachine = EntityStateMachine.FindByCustomName(this.projectileTransform.gameObject, "Arming");
			}
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x0002DACF File Offset: 0x0002BCCF
		private bool IsArmed()
		{
			EntityStateMachine entityStateMachine = this.armingStateMachine;
			BaseMineArmingState baseMineArmingState = ((entityStateMachine != null) ? entityStateMachine.state : null) as BaseMineArmingState;
			return ((baseMineArmingState != null) ? baseMineArmingState.damageScale : 0f) > 1f;
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x0002DAFF File Offset: 0x0002BCFF
		private void Update()
		{
			if (this.IsArmed())
			{
				this.animator.SetTrigger("Arming");
			}
		}

		// Token: 0x04000ACC RID: 2764
		private Transform projectileTransform;

		// Token: 0x04000ACD RID: 2765
		public Animator animator;

		// Token: 0x04000ACE RID: 2766
		private EntityStateMachine armingStateMachine;
	}
}
