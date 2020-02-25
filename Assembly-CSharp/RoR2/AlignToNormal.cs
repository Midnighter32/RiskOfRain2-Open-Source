using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000144 RID: 324
	public class AlignToNormal : MonoBehaviour
	{
		// Token: 0x060005C4 RID: 1476 RVA: 0x00017E28 File Offset: 0x00016028
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

		// Token: 0x0400063E RID: 1598
		[Tooltip("The amount to raycast down from.")]
		public float maxDistance;

		// Token: 0x0400063F RID: 1599
		[Tooltip("The amount to pull the object out of the ground initially to test.")]
		public float offsetDistance;

		// Token: 0x04000640 RID: 1600
		[Tooltip("Send to floor only - don't change normals.")]
		public bool changePositionOnly;
	}
}
