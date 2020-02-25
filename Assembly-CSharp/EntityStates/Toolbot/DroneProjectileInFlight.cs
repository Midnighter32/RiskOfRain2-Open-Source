using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x02000767 RID: 1895
	public class DroneProjectileInFlight : BaseState
	{
		// Token: 0x06002BBD RID: 11197 RVA: 0x000B9004 File Offset: 0x000B7204
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

		// Token: 0x06002BBE RID: 11198 RVA: 0x000B9094 File Offset: 0x000B7294
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

		// Token: 0x06002BBF RID: 11199 RVA: 0x000B90F9 File Offset: 0x000B72F9
		private void OnImpact(ProjectileImpactInfo projectileImpactInfo)
		{
			this.Advance();
		}

		// Token: 0x06002BC0 RID: 11200 RVA: 0x000B90F9 File Offset: 0x000B72F9
		private void OnFuse()
		{
			this.Advance();
		}

		// Token: 0x06002BC1 RID: 11201 RVA: 0x000B9104 File Offset: 0x000B7304
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

		// Token: 0x040027ED RID: 10221
		private ProjectileImpactEventCaller impactEventCaller;

		// Token: 0x040027EE RID: 10222
		private ProjectileSimple projectileSimple;

		// Token: 0x040027EF RID: 10223
		private ProjectileFuse projectileFuse;
	}
}
