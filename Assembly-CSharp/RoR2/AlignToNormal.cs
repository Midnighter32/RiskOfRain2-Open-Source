using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000253 RID: 595
	public class AlignToNormal : MonoBehaviour
	{
		// Token: 0x06000B1C RID: 2844 RVA: 0x000372FC File Offset: 0x000354FC
		private void Start()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + base.transform.up * this.offsetDistance, -base.transform.up, out raycastHit, this.maxDistance, LayerIndex.world.mask))
			{
				base.transform.position = raycastHit.point;
				if (!this.changePositionOnly)
				{
					base.transform.up = raycastHit.normal;
				}
			}
		}

		// Token: 0x04000F21 RID: 3873
		[Tooltip("The amount to raycast down from.")]
		public float maxDistance;

		// Token: 0x04000F22 RID: 3874
		[Tooltip("The amount to pull the object out of the ground initially to test.")]
		public float offsetDistance;

		// Token: 0x04000F23 RID: 3875
		[Tooltip("Send to floor only - don't change normals.")]
		public bool changePositionOnly;
	}
}
