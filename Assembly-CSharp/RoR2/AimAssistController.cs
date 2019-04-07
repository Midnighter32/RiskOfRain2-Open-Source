using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200024D RID: 589
	[RequireComponent(typeof(CameraRigController))]
	public class AimAssistController : MonoBehaviour
	{
		// Token: 0x06000B11 RID: 2833 RVA: 0x000370F2 File Offset: 0x000352F2
		private void Awake()
		{
			this.cameraRigController = base.GetComponent<CameraRigController>();
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x00004507 File Offset: 0x00002707
		private void CollectTargets()
		{
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x00037100 File Offset: 0x00035300
		private void Update()
		{
			this.cameraRigController.target;
		}

		// Token: 0x04000F10 RID: 3856
		private CameraRigController cameraRigController;

		// Token: 0x0200024E RID: 590
		private struct BullseyeDescriptor
		{
			// Token: 0x04000F11 RID: 3857
			public Vector3 position;

			// Token: 0x04000F12 RID: 3858
			public Quaternion rotation;

			// Token: 0x04000F13 RID: 3859
			public Vector3 scale;
		}

		// Token: 0x0200024F RID: 591
		private struct GenerateScreenSpaceDataJob : IJobParallelFor
		{
			// Token: 0x06000B15 RID: 2837 RVA: 0x00004507 File Offset: 0x00002707
			public void Execute(int index)
			{
			}

			// Token: 0x04000F14 RID: 3860
			[ReadOnly]
			public Rect screenCoords;

			// Token: 0x04000F15 RID: 3861
			[ReadOnly]
			public NativeArray<AimAssistController.GenerateScreenSpaceDataJob.Input> targetBuffer;

			// Token: 0x04000F16 RID: 3862
			public NativeArray<AimAssistController.GenerateScreenSpaceDataJob.Output> resultBuffer;

			// Token: 0x02000250 RID: 592
			public struct Input
			{
				// Token: 0x04000F17 RID: 3863
				public GameObject associatedObject;

				// Token: 0x04000F18 RID: 3864
				public AimAssistController.BullseyeDescriptor BullseyeDescriptor;
			}

			// Token: 0x02000251 RID: 593
			public struct Output
			{
				// Token: 0x04000F19 RID: 3865
				public float score;
			}
		}
	}
}
