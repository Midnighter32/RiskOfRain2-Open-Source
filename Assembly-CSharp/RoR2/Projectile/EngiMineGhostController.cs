using System;
using EntityStates.Engi.Mine;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x020004F2 RID: 1266
	[RequireComponent(typeof(ProjectileGhostController))]
	public class EngiMineGhostController : MonoBehaviour
	{
		// Token: 0x06001E1B RID: 7707 RVA: 0x00081C3D File Offset: 0x0007FE3D
		private void Awake()
		{
			this.projectileGhostController = base.GetComponent<ProjectileGhostController>();
			this.stickIndicator.SetActive(false);
		}

		// Token: 0x06001E1C RID: 7708 RVA: 0x00081C58 File Offset: 0x0007FE58
		private void Start()
		{
			Transform authorityTransform = this.projectileGhostController.authorityTransform;
			if (authorityTransform)
			{
				this.armingStateMachine = EntityStateMachine.FindByCustomName(authorityTransform.gameObject, "Arming");
			}
		}

		// Token: 0x06001E1D RID: 7709 RVA: 0x00081C8F File Offset: 0x0007FE8F
		private bool IsArmed()
		{
			EntityStateMachine entityStateMachine = this.armingStateMachine;
			BaseMineArmingState baseMineArmingState = ((entityStateMachine != null) ? entityStateMachine.state : null) as BaseMineArmingState;
			return ((baseMineArmingState != null) ? baseMineArmingState.damageScale : 0f) > 1f;
		}

		// Token: 0x06001E1E RID: 7710 RVA: 0x00081CC0 File Offset: 0x0007FEC0
		private void FixedUpdate()
		{
			bool flag = this.IsArmed();
			if (flag != this.cachedArmed)
			{
				this.cachedArmed = flag;
				this.stickIndicator.SetActive(flag);
			}
		}

		// Token: 0x04001B51 RID: 6993
		private ProjectileGhostController projectileGhostController;

		// Token: 0x04001B52 RID: 6994
		private EntityStateMachine armingStateMachine;

		// Token: 0x04001B53 RID: 6995
		[Tooltip("Child object which will be enabled if the projectile is armed.")]
		public GameObject stickIndicator;

		// Token: 0x04001B54 RID: 6996
		private bool cachedArmed;
	}
}
