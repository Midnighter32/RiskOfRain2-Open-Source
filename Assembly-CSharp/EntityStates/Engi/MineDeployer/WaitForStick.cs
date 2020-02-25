using System;
using RoR2.Projectile;

namespace EntityStates.Engi.MineDeployer
{
	// Token: 0x02000876 RID: 2166
	public class WaitForStick : BaseMineDeployerState
	{
		// Token: 0x060030D1 RID: 12497 RVA: 0x000D236B File Offset: 0x000D056B
		public override void OnEnter()
		{
			base.OnEnter();
			this.projectileStickOnImpact = base.GetComponent<ProjectileStickOnImpact>();
		}

		// Token: 0x060030D2 RID: 12498 RVA: 0x000D237F File Offset: 0x000D057F
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && this.projectileStickOnImpact.stuck)
			{
				this.outer.SetNextState(new FireMine());
			}
		}

		// Token: 0x04002F11 RID: 12049
		private ProjectileStickOnImpact projectileStickOnImpact;
	}
}
