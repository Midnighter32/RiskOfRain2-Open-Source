using System;
using System.Collections.Generic;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Engi.MineDeployer
{
	// Token: 0x02000873 RID: 2163
	public class BaseMineDeployerState : BaseState
	{
		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x060030C0 RID: 12480 RVA: 0x000D1EA8 File Offset: 0x000D00A8
		// (set) Token: 0x060030C1 RID: 12481 RVA: 0x000D1EB0 File Offset: 0x000D00B0
		public GameObject owner { get; private set; }

		// Token: 0x060030C2 RID: 12482 RVA: 0x000D1EB9 File Offset: 0x000D00B9
		public override void OnEnter()
		{
			base.OnEnter();
			ProjectileController projectileController = base.projectileController;
			this.owner = ((projectileController != null) ? projectileController.owner : null);
			BaseMineDeployerState.instancesList.Add(this);
		}

		// Token: 0x060030C3 RID: 12483 RVA: 0x000D1EE4 File Offset: 0x000D00E4
		public override void OnExit()
		{
			BaseMineDeployerState.instancesList.Remove(this);
			base.OnExit();
		}

		// Token: 0x04002F07 RID: 12039
		public static List<BaseMineDeployerState> instancesList = new List<BaseMineDeployerState>();
	}
}
