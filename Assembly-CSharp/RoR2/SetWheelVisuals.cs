using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000328 RID: 808
	[RequireComponent(typeof(WheelCollider))]
	public class SetWheelVisuals : MonoBehaviour
	{
		// Token: 0x06001312 RID: 4882 RVA: 0x00051BBA File Offset: 0x0004FDBA
		private void Start()
		{
			this.wheelCollider = base.GetComponent<WheelCollider>();
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x00051BC8 File Offset: 0x0004FDC8
		private void FixedUpdate()
		{
			Vector3 position;
			Quaternion rotation;
			this.wheelCollider.GetWorldPose(out position, out rotation);
			this.visualTransform.position = position;
			this.visualTransform.rotation = rotation;
		}

		// Token: 0x040011DC RID: 4572
		public Transform visualTransform;

		// Token: 0x040011DD RID: 4573
		private WheelCollider wheelCollider;
	}
}
