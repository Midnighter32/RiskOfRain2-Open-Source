using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace EntityStates.Engi.Mine
{
	// Token: 0x0200087E RID: 2174
	public class WaitForTarget : BaseMineState
	{
		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x060030EC RID: 12524 RVA: 0x0000B933 File Offset: 0x00009B33
		protected override bool shouldStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060030ED RID: 12525 RVA: 0x000D25B8 File Offset: 0x000D07B8
		public override void OnEnter()
		{
			base.OnEnter();
			this.projectileTargetComponent = base.GetComponent<ProjectileTargetComponent>();
			this.targetFinder = base.GetComponent<ProjectileSphereTargetFinder>();
			if (NetworkServer.active)
			{
				this.targetFinder.enabled = true;
				base.armingStateMachine.SetNextState(new MineArmingWeak());
			}
		}

		// Token: 0x060030EE RID: 12526 RVA: 0x000D2606 File Offset: 0x000D0806
		public override void OnExit()
		{
			if (this.targetFinder)
			{
				this.targetFinder.enabled = false;
			}
			base.OnExit();
		}

		// Token: 0x060030EF RID: 12527 RVA: 0x000D2628 File Offset: 0x000D0828
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.targetFinder)
			{
				if (this.projectileTargetComponent.target)
				{
					this.outer.SetNextState(new PreDetonate());
				}
				EntityStateMachine armingStateMachine = base.armingStateMachine;
				BaseMineArmingState baseMineArmingState;
				if ((baseMineArmingState = (((armingStateMachine != null) ? armingStateMachine.state : null) as BaseMineArmingState)) != null)
				{
					this.targetFinder.enabled = (baseMineArmingState.triggerRadius != 0f);
					this.targetFinder.lookRange = baseMineArmingState.triggerRadius;
				}
			}
		}

		// Token: 0x04002F1F RID: 12063
		private ProjectileSphereTargetFinder targetFinder;

		// Token: 0x04002F20 RID: 12064
		private ProjectileTargetComponent projectileTargetComponent;

		// Token: 0x04002F21 RID: 12065
		private ProjectileImpactExplosion projectileImpactExplosion;
	}
}
