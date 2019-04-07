using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000333 RID: 819
	public class IKTargetPassive : MonoBehaviour, IIKTargetBehavior
	{
		// Token: 0x060010CB RID: 4299 RVA: 0x00004507 File Offset: 0x00002707
		public void UpdateIKState(int targetState)
		{
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x00053CE7 File Offset: 0x00051EE7
		private void Awake()
		{
			if (this.cacheFirstPosition)
			{
				this.cachedLocalPosition = base.transform.localPosition;
			}
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x00053D04 File Offset: 0x00051F04
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

		// Token: 0x060010CE RID: 4302 RVA: 0x00053D58 File Offset: 0x00051F58
		public void UpdateIKTargetPosition()
		{
			this.ResetTransformToCachedPosition();
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + Vector3.up * -this.minHeight, Vector3.down, out raycastHit, this.maxHeight - this.minHeight, LayerIndex.world.mask))
			{
				this.targetHeightOffset = raycastHit.point.y - base.transform.position.y;
				return;
			}
			this.targetHeightOffset = 0f;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x00053DE8 File Offset: 0x00051FE8
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

		// Token: 0x060010D0 RID: 4304 RVA: 0x00053EAD File Offset: 0x000520AD
		private void ResetTransformToCachedPosition()
		{
			if (this.cacheFirstPosition)
			{
				base.transform.localPosition = new Vector3(this.cachedLocalPosition.x, this.cachedLocalPosition.y, this.cachedLocalPosition.z);
			}
		}

		// Token: 0x040014EB RID: 5355
		private float smoothedTargetHeightOffset;

		// Token: 0x040014EC RID: 5356
		private float targetHeightOffset;

		// Token: 0x040014ED RID: 5357
		private float smoothdampVelocity;

		// Token: 0x040014EE RID: 5358
		public float minHeight = -0.3f;

		// Token: 0x040014EF RID: 5359
		public float maxHeight = 1f;

		// Token: 0x040014F0 RID: 5360
		public float dampTime = 0.1f;

		// Token: 0x040014F1 RID: 5361
		[Tooltip("The IK weight float parameter if used")]
		public string animatorIKWeightFloat = "";

		// Token: 0x040014F2 RID: 5362
		public Animator animator;

		// Token: 0x040014F3 RID: 5363
		[Tooltip("The target transform will plant without any calls from external IK chains")]
		public bool selfPlant;

		// Token: 0x040014F4 RID: 5364
		public float selfPlantFrequency = 5f;

		// Token: 0x040014F5 RID: 5365
		[Tooltip("Whether or not to cache where the raycast begins. Used when not attached to bones, who reset themselves via animator.")]
		public bool cacheFirstPosition;

		// Token: 0x040014F6 RID: 5366
		private Vector3 cachedLocalPosition;

		// Token: 0x040014F7 RID: 5367
		private float selfPlantTimer;
	}
}
