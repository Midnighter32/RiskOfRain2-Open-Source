using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200052C RID: 1324
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class ProjectileSteerTowardTarget : MonoBehaviour
	{
		// Token: 0x06001F54 RID: 8020 RVA: 0x00088111 File Offset: 0x00086311
		private void Start()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.transform = base.transform;
			this.targetComponent = base.GetComponent<ProjectileTargetComponent>();
		}

		// Token: 0x06001F55 RID: 8021 RVA: 0x0008813C File Offset: 0x0008633C
		private void FixedUpdate()
		{
			if (this.targetComponent.target)
			{
				Vector3 vector = this.targetComponent.target.transform.position - this.transform.position;
				if (this.yAxisOnly)
				{
					vector.y = 0f;
				}
				if (vector != Vector3.zero)
				{
					this.transform.forward = Vector3.RotateTowards(this.transform.forward, vector, this.rotationSpeed * 0.017453292f * Time.fixedDeltaTime, 0f);
				}
			}
		}

		// Token: 0x04001CFC RID: 7420
		[Tooltip("Constrains rotation to the Y axis only.")]
		public bool yAxisOnly;

		// Token: 0x04001CFD RID: 7421
		[Tooltip("How fast to rotate in degrees per second. Rotation is linear.")]
		public float rotationSpeed;

		// Token: 0x04001CFE RID: 7422
		private new Transform transform;

		// Token: 0x04001CFF RID: 7423
		private ProjectileTargetComponent targetComponent;
	}
}
