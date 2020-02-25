using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000524 RID: 1316
	public class ProjectileSimple : MonoBehaviour
	{
		// Token: 0x06001F1B RID: 7963 RVA: 0x00087045 File Offset: 0x00085245
		private void Awake()
		{
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001F1C RID: 7964 RVA: 0x0008705F File Offset: 0x0008525F
		private void Start()
		{
			this.UpdateVelocity();
		}

		// Token: 0x06001F1D RID: 7965 RVA: 0x00087068 File Offset: 0x00085268
		private void UpdateVelocity()
		{
			if (this.rigidbody)
			{
				if (this.enableVelocityOverLifetime)
				{
					this.rigidbody.velocity = this.velocity * this.velocityOverLifetime.Evaluate(this.stopwatch / this.lifetime) * this.transform.forward;
					return;
				}
				this.rigidbody.velocity = this.transform.forward * this.velocity;
			}
		}

		// Token: 0x06001F1E RID: 7966 RVA: 0x000870E8 File Offset: 0x000852E8
		private void Update()
		{
			if (this.updateAfterFiring || this.enableVelocityOverLifetime)
			{
				this.UpdateVelocity();
			}
			this.stopwatch += Time.deltaTime;
			if (this.stopwatch > this.lifetime)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04001CBE RID: 7358
		public float velocity;

		// Token: 0x04001CBF RID: 7359
		public float lifetime = 5f;

		// Token: 0x04001CC0 RID: 7360
		public bool updateAfterFiring;

		// Token: 0x04001CC1 RID: 7361
		public bool enableVelocityOverLifetime;

		// Token: 0x04001CC2 RID: 7362
		public AnimationCurve velocityOverLifetime;

		// Token: 0x04001CC3 RID: 7363
		private float stopwatch;

		// Token: 0x04001CC4 RID: 7364
		private Rigidbody rigidbody;

		// Token: 0x04001CC5 RID: 7365
		private new Transform transform;
	}
}
