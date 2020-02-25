using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200014C RID: 332
	public class ApplyJiggleBoneMotion : MonoBehaviour
	{
		// Token: 0x060005E7 RID: 1511 RVA: 0x00018710 File Offset: 0x00016910
		private void FixedUpdate()
		{
			Vector3 position = this.rootTransform.position;
			Rigidbody[] array = this.rigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddForce((this.lastRootPosition - position) * this.forceScale * Time.fixedDeltaTime);
			}
			this.lastRootPosition = position;
		}

		// Token: 0x04000669 RID: 1641
		public float forceScale = 100f;

		// Token: 0x0400066A RID: 1642
		public Transform rootTransform;

		// Token: 0x0400066B RID: 1643
		public Rigidbody[] rigidbodies;

		// Token: 0x0400066C RID: 1644
		private Vector3 lastRootPosition;
	}
}
