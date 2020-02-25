using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000232 RID: 562
	public class HoverEngine : MonoBehaviour
	{
		// Token: 0x06000C8A RID: 3210 RVA: 0x00038688 File Offset: 0x00036888
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.TransformPoint(this.offsetVector), this.hoverRadius);
			Gizmos.DrawRay(this.castRay);
			if (this.isGrounded)
			{
				Gizmos.DrawSphere(this.raycastHit.point, this.hoverRadius);
			}
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x000386E4 File Offset: 0x000368E4
		private void FixedUpdate()
		{
			float num = Mathf.Clamp01(Vector3.Dot(-base.transform.up, Vector3.down));
			this.castPosition = base.transform.TransformPoint(this.offsetVector);
			this.castRay = new Ray(this.castPosition, -base.transform.up);
			this.isGrounded = false;
			this.forceStrength = 0f;
			this.compression = 0f;
			Vector3 position = this.castRay.origin + this.castRay.direction * this.hoverHeight;
			if (Physics.SphereCast(this.castRay, this.hoverRadius, out this.raycastHit, this.hoverHeight, LayerIndex.world.mask))
			{
				this.isGrounded = true;
				float num2 = (this.hoverHeight - this.raycastHit.distance) / this.hoverHeight;
				Vector3 a = Vector3.up * (num2 * this.hoverForce);
				Vector3 b = Vector3.Project(this.engineRigidbody.GetPointVelocity(this.castPosition), -base.transform.up) * this.hoverDamping;
				this.forceStrength = (a - b).magnitude;
				this.engineRigidbody.AddForceAtPosition(Vector3.Project(a - b, -base.transform.up), this.castPosition, ForceMode.Acceleration);
				this.compression = Mathf.Clamp01(num2 * num);
				position = this.raycastHit.point;
			}
			this.wheelVisual.position = position;
			bool flag = this.isGrounded;
		}

		// Token: 0x04000C7D RID: 3197
		public Rigidbody engineRigidbody;

		// Token: 0x04000C7E RID: 3198
		public Transform wheelVisual;

		// Token: 0x04000C7F RID: 3199
		public float hoverForce = 65f;

		// Token: 0x04000C80 RID: 3200
		public float hoverHeight = 3.5f;

		// Token: 0x04000C81 RID: 3201
		public float hoverDamping = 0.1f;

		// Token: 0x04000C82 RID: 3202
		public float hoverRadius;

		// Token: 0x04000C83 RID: 3203
		[HideInInspector]
		public float forceStrength;

		// Token: 0x04000C84 RID: 3204
		private Ray castRay;

		// Token: 0x04000C85 RID: 3205
		private Vector3 castPosition;

		// Token: 0x04000C86 RID: 3206
		[HideInInspector]
		public RaycastHit raycastHit;

		// Token: 0x04000C87 RID: 3207
		public float compression;

		// Token: 0x04000C88 RID: 3208
		public Vector3 offsetVector = Vector3.zero;

		// Token: 0x04000C89 RID: 3209
		public bool isGrounded;
	}
}
