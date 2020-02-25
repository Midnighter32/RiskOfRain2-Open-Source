using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiBubbleShield
{
	// Token: 0x02000892 RID: 2194
	public class Undeployed : EntityState
	{
		// Token: 0x0600313F RID: 12607 RVA: 0x000D4124 File Offset: 0x000D2324
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

		// Token: 0x06003140 RID: 12608 RVA: 0x000D418E File Offset: 0x000D238E
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.projectileStickOnImpact.stuck && NetworkServer.active)
			{
				this.SetNextState();
			}
		}

		// Token: 0x06003141 RID: 12609 RVA: 0x000D41B0 File Offset: 0x000D23B0
		protected virtual void SetNextState()
		{
			this.outer.SetNextState(new Deployed());
		}

		// Token: 0x04002F93 RID: 12179
		private ProjectileStickOnImpact projectileStickOnImpact;
	}
}
