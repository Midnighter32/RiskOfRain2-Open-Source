using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000388 RID: 904
	public class PlacementUtility : MonoBehaviour
	{
		// Token: 0x060012E1 RID: 4833 RVA: 0x00004507 File Offset: 0x00002707
		private void Start()
		{
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x00004507 File Offset: 0x00002707
		private void Update()
		{
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0005C9F9 File Offset: 0x0005ABF9
		public void PlacePrefab(Vector3 targetPosition, Quaternion rotation)
		{
			this.prefabPlacement;
		}

		// Token: 0x040016AC RID: 5804
		public Transform targetParent;

		// Token: 0x040016AD RID: 5805
		public GameObject prefabPlacement;

		// Token: 0x040016AE RID: 5806
		public bool normalToSurface;

		// Token: 0x040016AF RID: 5807
		public bool flipForwardDirection;

		// Token: 0x040016B0 RID: 5808
		public float minScale = 1f;

		// Token: 0x040016B1 RID: 5809
		public float maxScale = 2f;

		// Token: 0x040016B2 RID: 5810
		public float minXRotation;

		// Token: 0x040016B3 RID: 5811
		public float maxXRotation;

		// Token: 0x040016B4 RID: 5812
		public float minYRotation;

		// Token: 0x040016B5 RID: 5813
		public float maxYRotation;

		// Token: 0x040016B6 RID: 5814
		public float minZRotation;

		// Token: 0x040016B7 RID: 5815
		public float maxZRotation;
	}
}
