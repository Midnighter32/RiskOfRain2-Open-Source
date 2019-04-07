using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025B RID: 603
	public class ApplyJiggleBoneMotion : MonoBehaviour
	{
		// Token: 0x06000B3E RID: 2878 RVA: 0x00037B9C File Offset: 0x00035D9C
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

		// Token: 0x04000F4C RID: 3916
		public float forceScale = 100f;

		// Token: 0x04000F4D RID: 3917
		public Transform rootTransform;

		// Token: 0x04000F4E RID: 3918
		public Rigidbody[] rigidbodies;

		// Token: 0x04000F4F RID: 3919
		private Vector3 lastRootPosition;
	}
}
