using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x020000E2 RID: 226
	public class DroneProjectileHover : BaseState
	{
		// Token: 0x0600046A RID: 1130 RVA: 0x000128BC File Offset: 0x00010ABC
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.rigidbody)
			{
				base.rigidbody.velocity = Vector3.zero;
				base.rigidbody.useGravity = false;
			}
			if (NetworkServer.active)
			{
				this.projectileController = base.GetComponent<ProjectileController>();
				if (this.projectileController)
				{
					this.teamFilter = this.projectileController.teamFilter;
				}
			}
			this.interval = DroneProjectileHover.duration / (float)(DroneProjectileHover.pulseCount + 1);
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00012940 File Offset: 0x00010B40
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				if (base.age >= DroneProjectileHover.duration)
				{
					EntityState.Destroy(base.gameObject);
					return;
				}
				if (base.age >= this.interval * (float)(this.pulses + 1))
				{
					this.pulses++;
					this.Pulse();
				}
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00004507 File Offset: 0x00002707
		protected virtual void Pulse()
		{
		}

		// Token: 0x04000436 RID: 1078
		public static float duration;

		// Token: 0x04000437 RID: 1079
		public static int pulseCount = 3;

		// Token: 0x04000438 RID: 1080
		public static float pulseRadius = 7f;

		// Token: 0x04000439 RID: 1081
		protected ProjectileController projectileController;

		// Token: 0x0400043A RID: 1082
		protected TeamFilter teamFilter;

		// Token: 0x0400043B RID: 1083
		protected float interval;

		// Token: 0x0400043C RID: 1084
		protected int pulses;
	}
}
