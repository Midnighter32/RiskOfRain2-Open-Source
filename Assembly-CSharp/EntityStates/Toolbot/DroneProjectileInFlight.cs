using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x020000E0 RID: 224
	public class DroneProjectileInFlight : BaseState
	{
		// Token: 0x06000462 RID: 1122 RVA: 0x0001270C File Offset: 0x0001090C
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				this.impactEventCaller = base.GetComponent<ProjectileImpactEventCaller>();
				if (this.impactEventCaller)
				{
					this.impactEventCaller.impactEvent.AddListener(new UnityAction<ProjectileImpactInfo>(this.OnImpact));
				}
				this.projectileSimple = base.GetComponent<ProjectileSimple>();
				this.projectileFuse = base.GetComponent<ProjectileFuse>();
				if (this.projectileFuse)
				{
					this.projectileFuse.onFuse.AddListener(new UnityAction(this.OnFuse));
				}
			}
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0001279C File Offset: 0x0001099C
		public override void OnExit()
		{
			if (this.impactEventCaller)
			{
				this.impactEventCaller.impactEvent.RemoveListener(new UnityAction<ProjectileImpactInfo>(this.OnImpact));
			}
			if (this.projectileFuse)
			{
				this.projectileFuse.onFuse.RemoveListener(new UnityAction(this.OnFuse));
			}
			base.OnEnter();
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00012801 File Offset: 0x00010A01
		private void OnImpact(ProjectileImpactInfo projectileImpactInfo)
		{
			this.Advance();
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x00012801 File Offset: 0x00010A01
		private void OnFuse()
		{
			this.Advance();
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x0001280C File Offset: 0x00010A0C
		private void Advance()
		{
			if (NetworkServer.active)
			{
				if (this.projectileSimple)
				{
					this.projectileSimple.velocity = 0f;
					this.projectileSimple.enabled = false;
				}
				if (base.rigidbody)
				{
					base.rigidbody.velocity = new Vector3(0f, Trajectory.CalculateInitialYSpeedForFlightDuration(DroneProjectilePrepHover.duration), 0f);
				}
			}
			if (base.isAuthority)
			{
				this.outer.SetNextState(new DroneProjectilePrepHover());
			}
		}

		// Token: 0x04000432 RID: 1074
		private ProjectileImpactEventCaller impactEventCaller;

		// Token: 0x04000433 RID: 1075
		private ProjectileSimple projectileSimple;

		// Token: 0x04000434 RID: 1076
		private ProjectileFuse projectileFuse;
	}
}
