using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x0200090D RID: 2317
	[RequireComponent(typeof(Rigidbody))]
	public class BasicPhysicsMoverController : BaseMoverController
	{
		// Token: 0x060033D6 RID: 13270 RVA: 0x000E0CE8 File Offset: 0x000DEEE8
		public override void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
		{
			goalPosition = this.referenceTransformPosition.position;
			goalRotation = this.referenceTransformPosition.rotation;
		}

		// Token: 0x060033D7 RID: 13271 RVA: 0x000E0D0C File Offset: 0x000DEF0C
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x060033D8 RID: 13272 RVA: 0x0000409B File Offset: 0x0000229B
		private void Update()
		{
		}

		// Token: 0x04003342 RID: 13122
		public Transform referenceTransformPosition;

		// Token: 0x04003343 RID: 13123
		private Rigidbody rb;
	}
}
