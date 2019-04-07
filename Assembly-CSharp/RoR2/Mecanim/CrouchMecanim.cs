using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000567 RID: 1383
	public class CrouchMecanim : MonoBehaviour
	{
		// Token: 0x06001ED7 RID: 7895 RVA: 0x000919B3 File Offset: 0x0008FBB3
		private void Awake()
		{
			this.crouchLayer = this.animator.GetLayerIndex("Crouch, Additive");
		}

		// Token: 0x06001ED8 RID: 7896 RVA: 0x000919CC File Offset: 0x0008FBCC
		private void FixedUpdate()
		{
			this.crouchStopwatch -= Time.fixedDeltaTime;
			if (this.crouchStopwatch <= 0f)
			{
				this.crouchStopwatch = 0.5f;
				RaycastHit raycastHit;
				bool flag;
				if (!this.crouchOriginOverride)
				{
					flag = Physics.Raycast(new Ray(base.transform.position - base.transform.up * this.initialVerticalOffset, base.transform.up), out raycastHit, this.duckHeight + this.initialVerticalOffset, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
				}
				else
				{
					flag = Physics.Raycast(new Ray(this.crouchOriginOverride.position - this.crouchOriginOverride.up * this.initialVerticalOffset, this.crouchOriginOverride.up), out raycastHit, this.duckHeight + this.initialVerticalOffset, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
				}
				this.crouchCycle = 0f;
				if (flag)
				{
					this.crouchCycle = Mathf.Clamp01(1f - (raycastHit.distance - this.initialVerticalOffset) / this.duckHeight);
				}
			}
		}

		// Token: 0x06001ED9 RID: 7897 RVA: 0x00091B05 File Offset: 0x0008FD05
		private void Update()
		{
			this.animator.SetFloat("crouchCycleOffset", this.crouchCycle, this.smoothdamp, Time.deltaTime);
		}

		// Token: 0x06001EDA RID: 7898 RVA: 0x00091B28 File Offset: 0x0008FD28
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.up * this.duckHeight);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, base.transform.position + -base.transform.up * this.initialVerticalOffset);
		}

		// Token: 0x04002189 RID: 8585
		public float duckHeight;

		// Token: 0x0400218A RID: 8586
		public Animator animator;

		// Token: 0x0400218B RID: 8587
		public float smoothdamp;

		// Token: 0x0400218C RID: 8588
		public float initialVerticalOffset;

		// Token: 0x0400218D RID: 8589
		public Transform crouchOriginOverride;

		// Token: 0x0400218E RID: 8590
		private int crouchLayer;

		// Token: 0x0400218F RID: 8591
		private float crouchCycle;

		// Token: 0x04002190 RID: 8592
		private const float crouchRaycastFrequency = 2f;

		// Token: 0x04002191 RID: 8593
		private float crouchStopwatch;
	}
}
