using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D7 RID: 983
	[RequireComponent(typeof(WheelCollider))]
	public class SetWheelVisuals : MonoBehaviour
	{
		// Token: 0x0600154E RID: 5454 RVA: 0x000663CA File Offset: 0x000645CA
		private void Start()
		{
			this.wheelCollider = base.GetComponent<WheelCollider>();
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x000663D8 File Offset: 0x000645D8
		private void FixedUpdate()
		{
			Vector3 position;
			Quaternion rotation;
			this.wheelCollider.GetWorldPose(out position, out rotation);
			this.visualTransform.position = position;
			this.visualTransform.rotation = rotation;
		}

		// Token: 0x040018A1 RID: 6305
		public Transform visualTransform;

		// Token: 0x040018A2 RID: 6306
		private WheelCollider wheelCollider;
	}
}
