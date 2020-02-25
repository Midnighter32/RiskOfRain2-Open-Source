using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200013E RID: 318
	[RequireComponent(typeof(CameraRigController))]
	public class AimAssistController : MonoBehaviour
	{
		// Token: 0x060005B9 RID: 1465 RVA: 0x00017C1E File Offset: 0x00015E1E
		private void Awake()
		{
			this.cameraRigController = base.GetComponent<CameraRigController>();
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x0000409B File Offset: 0x0000229B
		private void CollectTargets()
		{
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x00017C2C File Offset: 0x00015E2C
		private void Update()
		{
			this.cameraRigController.target;
		}

		// Token: 0x0400062D RID: 1581
		private CameraRigController cameraRigController;

		// Token: 0x0200013F RID: 319
		private struct BullseyeDescriptor
		{
			// Token: 0x0400062E RID: 1582
			public Vector3 position;

			// Token: 0x0400062F RID: 1583
			public Quaternion rotation;

			// Token: 0x04000630 RID: 1584
			public Vector3 scale;
		}

		// Token: 0x02000140 RID: 320
		private struct GenerateScreenSpaceDataJob : IJobParallelFor
		{
			// Token: 0x060005BD RID: 1469 RVA: 0x0000409B File Offset: 0x0000229B
			public void Execute(int index)
			{
			}

			// Token: 0x04000631 RID: 1585
			[ReadOnly]
			public Rect screenCoords;

			// Token: 0x04000632 RID: 1586
			[ReadOnly]
			public NativeArray<AimAssistController.GenerateScreenSpaceDataJob.Input> targetBuffer;

			// Token: 0x04000633 RID: 1587
			public NativeArray<AimAssistController.GenerateScreenSpaceDataJob.Output> resultBuffer;

			// Token: 0x02000141 RID: 321
			public struct Input
			{
				// Token: 0x04000634 RID: 1588
				public GameObject associatedObject;

				// Token: 0x04000635 RID: 1589
				public AimAssistController.BullseyeDescriptor BullseyeDescriptor;
			}

			// Token: 0x02000142 RID: 322
			public struct Output
			{
				// Token: 0x04000636 RID: 1590
				public float score;
			}
		}
	}
}
