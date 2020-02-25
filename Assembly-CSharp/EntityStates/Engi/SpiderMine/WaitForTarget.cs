using System;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace EntityStates.Engi.SpiderMine
{
	// Token: 0x0200086D RID: 2157
	public class WaitForTarget : BaseSpiderMineState
	{
		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x060030A2 RID: 12450 RVA: 0x000D18C1 File Offset: 0x000CFAC1
		// (set) Token: 0x060030A3 RID: 12451 RVA: 0x000D18C9 File Offset: 0x000CFAC9
		private protected ProjectileSphereTargetFinder targetFinder { protected get; private set; }

		// Token: 0x060030A4 RID: 12452 RVA: 0x000D18D2 File Offset: 0x000CFAD2
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				this.targetFinder = base.GetComponent<ProjectileSphereTargetFinder>();
				this.targetFinder.enabled = true;
			}
			base.PlayAnimation("Base", "Armed");
		}

		// Token: 0x060030A5 RID: 12453 RVA: 0x000D190C File Offset: 0x000CFB0C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				EntityState entityState = null;
				if (!base.projectileStickOnImpact.stuck)
				{
					entityState = new WaitForStick();
				}
				else if (base.projectileTargetComponent.target)
				{
					entityState = new Unburrow();
				}
				if (entityState != null)
				{
					this.outer.SetNextState(entityState);
				}
			}
		}

		// Token: 0x060030A6 RID: 12454 RVA: 0x000D1965 File Offset: 0x000CFB65
		public override void OnExit()
		{
			if (this.targetFinder)
			{
				this.targetFinder.enabled = false;
			}
			base.OnExit();
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x060030A7 RID: 12455 RVA: 0x0000B933 File Offset: 0x00009B33
		protected override bool shouldStick
		{
			get
			{
				return true;
			}
		}
	}
}
