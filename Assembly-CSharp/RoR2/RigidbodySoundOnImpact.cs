using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E6 RID: 742
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class RigidbodySoundOnImpact : MonoBehaviour
	{
		// Token: 0x060010FA RID: 4346 RVA: 0x0004AC1F File Offset: 0x00048E1F
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0004AC2D File Offset: 0x00048E2D
		private void FixedUpdate()
		{
			this.ditherTimer -= Time.fixedDeltaTime;
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x0004AC44 File Offset: 0x00048E44
		private void OnCollisionEnter(Collision collision)
		{
			if (this.ditherTimer > 0f)
			{
				return;
			}
			if (this.rb.isKinematic)
			{
				return;
			}
			if (collision.transform.gameObject.layer != LayerIndex.world.intVal)
			{
				return;
			}
			if (collision.relativeVelocity.sqrMagnitude > this.minimumRelativeVelocityMagnitude * this.minimumRelativeVelocityMagnitude)
			{
				collision.GetContact(0);
				Util.PlaySound(this.impactSoundString, base.gameObject);
				this.ditherTimer = 0.5f;
			}
		}

		// Token: 0x0400106E RID: 4206
		private Rigidbody rb;

		// Token: 0x0400106F RID: 4207
		public string impactSoundString;

		// Token: 0x04001070 RID: 4208
		public float minimumRelativeVelocityMagnitude;

		// Token: 0x04001071 RID: 4209
		private float ditherTimer;
	}
}
