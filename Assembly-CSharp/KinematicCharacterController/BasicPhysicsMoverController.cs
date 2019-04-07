using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006C2 RID: 1730
	[RequireComponent(typeof(Rigidbody))]
	public class BasicPhysicsMoverController : BaseMoverController
	{
		// Token: 0x0600268C RID: 9868 RVA: 0x000B18A0 File Offset: 0x000AFAA0
		public override void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
		{
			goalPosition = this.referenceTransformPosition.position;
			goalRotation = this.referenceTransformPosition.rotation;
		}

		// Token: 0x0600268D RID: 9869 RVA: 0x000B18C4 File Offset: 0x000AFAC4
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x0600268E RID: 9870 RVA: 0x00004507 File Offset: 0x00002707
		private void Update()
		{
		}

		// Token: 0x040028A9 RID: 10409
		public Transform referenceTransformPosition;

		// Token: 0x040028AA RID: 10410
		private Rigidbody rb;
	}
}
