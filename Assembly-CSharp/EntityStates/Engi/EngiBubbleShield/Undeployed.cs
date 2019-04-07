using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiBubbleShield
{
	// Token: 0x0200018F RID: 399
	public class Undeployed : EntityState
	{
		// Token: 0x060007AF RID: 1967 RVA: 0x00026290 File Offset: 0x00024490
		public override void OnEnter()
		{
			base.OnEnter();
			ProjectileController component = base.GetComponent<ProjectileController>();
			this.projectileStickOnImpact = base.GetComponent<ProjectileStickOnImpact>();
			if (NetworkServer.active && component.owner)
			{
				CharacterBody component2 = component.owner.GetComponent<CharacterBody>();
				if (component2)
				{
					CharacterMaster master = component2.master;
					if (master)
					{
						master.AddDeployable(base.GetComponent<Deployable>(), DeployableSlot.EngiBubbleShield);
					}
				}
			}
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x000262FA File Offset: 0x000244FA
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.projectileStickOnImpact.stuck && NetworkServer.active)
			{
				this.outer.SetNextState(new Deployed());
			}
		}

		// Token: 0x040009F7 RID: 2551
		private ProjectileStickOnImpact projectileStickOnImpact;
	}
}
