using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000562 RID: 1378
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class ProjectileSteerTowardTarget : MonoBehaviour
	{
		// Token: 0x06001EC6 RID: 7878 RVA: 0x00091540 File Offset: 0x0008F740
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

		// Token: 0x06001EC7 RID: 7879 RVA: 0x0009156C File Offset: 0x0008F76C
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

		// Token: 0x04002172 RID: 8562
		[Tooltip("Constrains rotation to the Y axis only.")]
		public bool yAxisOnly;

		// Token: 0x04002173 RID: 8563
		[Tooltip("How fast to rotate in degrees per second. Rotation is linear.")]
		public float rotationSpeed;

		// Token: 0x04002174 RID: 8564
		private new Transform transform;

		// Token: 0x04002175 RID: 8565
		private ProjectileTargetComponent targetComponent;
	}
}
