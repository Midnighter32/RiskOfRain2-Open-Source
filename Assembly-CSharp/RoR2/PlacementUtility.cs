using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002C0 RID: 704
	public class PlacementUtility : MonoBehaviour
	{
		// Token: 0x06000FED RID: 4077 RVA: 0x0000409B File Offset: 0x0000229B
		private void Start()
		{
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x0000409B File Offset: 0x0000229B
		private void Update()
		{
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x000460F9 File Offset: 0x000442F9
		public void PlacePrefab(Vector3 targetPosition, Quaternion rotation)
		{
			this.prefabPlacement;
		}

		// Token: 0x04000F69 RID: 3945
		public Transform targetParent;

		// Token: 0x04000F6A RID: 3946
		public GameObject prefabPlacement;

		// Token: 0x04000F6B RID: 3947
		public bool normalToSurface;

		// Token: 0x04000F6C RID: 3948
		public bool flipForwardDirection;

		// Token: 0x04000F6D RID: 3949
		public float minScale = 1f;

		// Token: 0x04000F6E RID: 3950
		public float maxScale = 2f;

		// Token: 0x04000F6F RID: 3951
		public float minXRotation;

		// Token: 0x04000F70 RID: 3952
		public float maxXRotation;

		// Token: 0x04000F71 RID: 3953
		public float minYRotation;

		// Token: 0x04000F72 RID: 3954
		public float maxYRotation;

		// Token: 0x04000F73 RID: 3955
		public float minZRotation;

		// Token: 0x04000F74 RID: 3956
		public float maxZRotation;
	}
}
