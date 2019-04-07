using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000312 RID: 786
	public class HoverEngine : MonoBehaviour
	{
		// Token: 0x06001048 RID: 4168 RVA: 0x000518CC File Offset: 0x0004FACC
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

		// Token: 0x06001049 RID: 4169 RVA: 0x00051928 File Offset: 0x0004FB28
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

		// Token: 0x0400143F RID: 5183
		public Rigidbody engineRigidbody;

		// Token: 0x04001440 RID: 5184
		public Transform wheelVisual;

		// Token: 0x04001441 RID: 5185
		public float hoverForce = 65f;

		// Token: 0x04001442 RID: 5186
		public float hoverHeight = 3.5f;

		// Token: 0x04001443 RID: 5187
		public float hoverDamping = 0.1f;

		// Token: 0x04001444 RID: 5188
		public float hoverRadius;

		// Token: 0x04001445 RID: 5189
		[HideInInspector]
		public float forceStrength;

		// Token: 0x04001446 RID: 5190
		private Ray castRay;

		// Token: 0x04001447 RID: 5191
		private Vector3 castPosition;

		// Token: 0x04001448 RID: 5192
		[HideInInspector]
		public RaycastHit raycastHit;

		// Token: 0x04001449 RID: 5193
		public float compression;

		// Token: 0x0400144A RID: 5194
		public Vector3 offsetVector = Vector3.zero;

		// Token: 0x0400144B RID: 5195
		public bool isGrounded;
	}
}
