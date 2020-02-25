using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.Mine
{
	// Token: 0x0200087B RID: 2171
	public class BaseMineState : BaseState
	{
		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x060030DB RID: 12507 RVA: 0x000D2473 File Offset: 0x000D0673
		// (set) Token: 0x060030DC RID: 12508 RVA: 0x000D247B File Offset: 0x000D067B
		private protected ProjectileStickOnImpact projectileStickOnImpact { protected get; private set; }

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x060030DD RID: 12509 RVA: 0x000D2484 File Offset: 0x000D0684
		// (set) Token: 0x060030DE RID: 12510 RVA: 0x000D248C File Offset: 0x000D068C
		private protected EntityStateMachine armingStateMachine { protected get; private set; }

		// Token: 0x060030DF RID: 12511 RVA: 0x000D2498 File Offset: 0x000D0698
		public override void OnEnter()
		{
			base.OnEnter();
			this.projectileStickOnImpact = base.GetComponent<ProjectileStickOnImpact>();
			this.armingStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Arming");
			if (this.projectileStickOnImpact.enabled != this.shouldStick)
			{
				this.projectileStickOnImpact.enabled = this.shouldStick;
			}
			Util.PlaySound(this.enterSoundString, base.gameObject);
		}

		// Token: 0x060030E0 RID: 12512 RVA: 0x000D2503 File Offset: 0x000D0703
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.shouldRevertToWaitForStickOnSurfaceLost && !this.projectileStickOnImpact.stuck)
			{
				this.outer.SetNextState(new WaitForStick());
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x060030E1 RID: 12513 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected virtual bool shouldStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x060030E2 RID: 12514 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected virtual bool shouldRevertToWaitForStickOnSurfaceLost
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002F1D RID: 12061
		[SerializeField]
		public string enterSoundString;
	}
}
