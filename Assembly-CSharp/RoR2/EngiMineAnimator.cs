using System;
using RoR2.Projectile;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E2 RID: 738
	public class EngiMineAnimator : MonoBehaviour
	{
		// Token: 0x06000EC9 RID: 3785 RVA: 0x00048BF4 File Offset: 0x00046DF4
		private void Start()
		{
			ProjectileGhostController component = base.GetComponent<ProjectileGhostController>();
			if (component)
			{
				this.projectileTransform = component.authorityTransform;
				this.engiMineController = this.projectileTransform.GetComponent<EngiMineController>();
				this.animator = base.GetComponentInChildren<Animator>();
			}
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x00048C39 File Offset: 0x00046E39
		private void Update()
		{
			if (this.projectileTransform && this.engiMineController.mineState == EngiMineController.MineState.Priming && this.animator)
			{
				this.animator.SetTrigger("Arming");
			}
		}

		// Token: 0x040012D7 RID: 4823
		private Transform projectileTransform;

		// Token: 0x040012D8 RID: 4824
		private EngiMineController engiMineController;

		// Token: 0x040012D9 RID: 4825
		private Animator animator;
	}
}
