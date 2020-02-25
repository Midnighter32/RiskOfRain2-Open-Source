using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000255 RID: 597
	public class IKTargetPassive : MonoBehaviour, IIKTargetBehavior
	{
		// Token: 0x06000D18 RID: 3352 RVA: 0x0000409B File Offset: 0x0000229B
		public void UpdateIKState(int targetState)
		{
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x0003AC8B File Offset: 0x00038E8B
		private void Awake()
		{
			if (this.cacheFirstPosition)
			{
				this.cachedLocalPosition = base.transform.localPosition;
			}
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x0003ACA8 File Offset: 0x00038EA8
		private void LateUpdate()
		{
			this.selfPlantTimer -= Time.deltaTime;
			if (this.selfPlant && this.selfPlantTimer <= 0f)
			{
				this.selfPlantTimer = 1f / this.selfPlantFrequency;
				this.UpdateIKTargetPosition();
			}
			this.UpdateYOffset();
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x0003ACFC File Offset: 0x00038EFC
		public void UpdateIKTargetPosition()
		{
			this.ResetTransformToCachedPosition();
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + Vector3.up * -this.minHeight, Vector3.down, out raycastHit, this.maxHeight - this.minHeight, LayerIndex.world.mask))
			{
				this.targetHeightOffset = raycastHit.point.y - base.transform.position.y;
			}
			else
			{
				this.targetHeightOffset = 0f;
			}
			this.targetHeightOffset += this.baseOffset;
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x0003ADA0 File Offset: 0x00038FA0
		public void UpdateYOffset()
		{
			float t = 1f;
			if (this.animator && this.animatorIKWeightFloat.Length > 0)
			{
				t = this.animator.GetFloat(this.animatorIKWeightFloat);
			}
			this.smoothedTargetHeightOffset = Mathf.SmoothDamp(this.smoothedTargetHeightOffset, this.targetHeightOffset, ref this.smoothdampVelocity, this.dampTime, float.PositiveInfinity, Time.deltaTime);
			this.ResetTransformToCachedPosition();
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + Mathf.Lerp(0f, this.smoothedTargetHeightOffset, t), base.transform.position.z);
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0003AE65 File Offset: 0x00039065
		private void ResetTransformToCachedPosition()
		{
			if (this.cacheFirstPosition)
			{
				base.transform.localPosition = new Vector3(this.cachedLocalPosition.x, this.cachedLocalPosition.y, this.cachedLocalPosition.z);
			}
		}

		// Token: 0x04000D2E RID: 3374
		private float smoothedTargetHeightOffset;

		// Token: 0x04000D2F RID: 3375
		private float targetHeightOffset;

		// Token: 0x04000D30 RID: 3376
		private float smoothdampVelocity;

		// Token: 0x04000D31 RID: 3377
		public float minHeight = -0.3f;

		// Token: 0x04000D32 RID: 3378
		public float maxHeight = 1f;

		// Token: 0x04000D33 RID: 3379
		public float dampTime = 0.1f;

		// Token: 0x04000D34 RID: 3380
		public float baseOffset;

		// Token: 0x04000D35 RID: 3381
		[Tooltip("The IK weight float parameter if used")]
		public string animatorIKWeightFloat = "";

		// Token: 0x04000D36 RID: 3382
		public Animator animator;

		// Token: 0x04000D37 RID: 3383
		[Tooltip("The target transform will plant without any calls from external IK chains")]
		public bool selfPlant;

		// Token: 0x04000D38 RID: 3384
		public float selfPlantFrequency = 5f;

		// Token: 0x04000D39 RID: 3385
		[Tooltip("Whether or not to cache where the raycast begins. Used when not attached to bones, who reset themselves via animator.")]
		public bool cacheFirstPosition;

		// Token: 0x04000D3A RID: 3386
		private Vector3 cachedLocalPosition;

		// Token: 0x04000D3B RID: 3387
		private float selfPlantTimer;
	}
}
