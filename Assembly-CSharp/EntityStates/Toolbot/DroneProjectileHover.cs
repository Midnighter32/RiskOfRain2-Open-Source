using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x02000769 RID: 1897
	public class DroneProjectileHover : BaseState
	{
		// Token: 0x06002BC5 RID: 11205 RVA: 0x000B91B4 File Offset: 0x000B73B4
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.rigidbody)
			{
				base.rigidbody.velocity = Vector3.zero;
				base.rigidbody.useGravity = false;
			}
			if (NetworkServer.active && base.projectileController)
			{
				this.teamFilter = base.projectileController.teamFilter;
			}
			this.interval = this.duration / (float)(this.pulseCount + 1);
		}

		// Token: 0x06002BC6 RID: 11206 RVA: 0x000B922C File Offset: 0x000B742C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				if (base.age >= this.duration)
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

		// Token: 0x06002BC7 RID: 11207 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void Pulse()
		{
		}

		// Token: 0x040027F1 RID: 10225
		[SerializeField]
		public float duration;

		// Token: 0x040027F2 RID: 10226
		[SerializeField]
		public int pulseCount = 3;

		// Token: 0x040027F3 RID: 10227
		[SerializeField]
		public float pulseRadius = 7f;

		// Token: 0x040027F4 RID: 10228
		protected TeamFilter teamFilter;

		// Token: 0x040027F5 RID: 10229
		protected float interval;

		// Token: 0x040027F6 RID: 10230
		protected int pulses;
	}
}
