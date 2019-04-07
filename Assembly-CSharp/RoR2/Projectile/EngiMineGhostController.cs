using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000538 RID: 1336
	[RequireComponent(typeof(ProjectileGhostController))]
	public class EngiMineGhostController : MonoBehaviour
	{
		// Token: 0x06001DE6 RID: 7654 RVA: 0x0008C958 File Offset: 0x0008AB58
		private EngiMineController LookupMineController()
		{
			if (!this.cachedMineController)
			{
				Transform authorityTransform = this.projectileGhostController.authorityTransform;
				if (authorityTransform)
				{
					this.cachedMineController = authorityTransform.GetComponent<EngiMineController>();
				}
			}
			return this.cachedMineController;
		}

		// Token: 0x06001DE7 RID: 7655 RVA: 0x0008C998 File Offset: 0x0008AB98
		private void Awake()
		{
			this.projectileGhostController = base.GetComponent<ProjectileGhostController>();
			this.stickIndicator.SetActive(false);
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x0008C9B4 File Offset: 0x0008ABB4
		private void FixedUpdate()
		{
			bool flag = false;
			EngiMineController engiMineController = this.LookupMineController();
			if (engiMineController)
			{
				flag = (engiMineController.mineState == EngiMineController.MineState.Sticking);
			}
			if (flag != this.cachedArmed)
			{
				this.cachedArmed = flag;
				this.stickIndicator.SetActive(flag);
			}
		}

		// Token: 0x04002038 RID: 8248
		private ProjectileGhostController projectileGhostController;

		// Token: 0x04002039 RID: 8249
		[Tooltip("Child object which will be enabled if the projectile is armed.")]
		public GameObject stickIndicator;

		// Token: 0x0400203A RID: 8250
		private EngiMineController cachedMineController;

		// Token: 0x0400203B RID: 8251
		private bool cachedArmed;
	}
}
