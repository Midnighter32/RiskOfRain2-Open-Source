using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200055E RID: 1374
	public class ProjectileSimple : MonoBehaviour
	{
		// Token: 0x06001EAA RID: 7850 RVA: 0x00090BF2 File Offset: 0x0008EDF2
		private void Awake()
		{
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001EAB RID: 7851 RVA: 0x00090C0C File Offset: 0x0008EE0C
		private void Start()
		{
			this.UpdateVelocity();
		}

		// Token: 0x06001EAC RID: 7852 RVA: 0x00090C14 File Offset: 0x0008EE14
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

		// Token: 0x06001EAD RID: 7853 RVA: 0x00090C94 File Offset: 0x0008EE94
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

		// Token: 0x04002155 RID: 8533
		public float velocity;

		// Token: 0x04002156 RID: 8534
		public float lifetime = 5f;

		// Token: 0x04002157 RID: 8535
		public bool updateAfterFiring;

		// Token: 0x04002158 RID: 8536
		public bool enableVelocityOverLifetime;

		// Token: 0x04002159 RID: 8537
		public AnimationCurve velocityOverLifetime;

		// Token: 0x0400215A RID: 8538
		private float stopwatch;

		// Token: 0x0400215B RID: 8539
		private Rigidbody rigidbody;

		// Token: 0x0400215C RID: 8540
		private new Transform transform;
	}
}
