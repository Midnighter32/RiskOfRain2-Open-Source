using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000256 RID: 598
	public class IKTargetPlant : MonoBehaviour, IIKTargetBehavior
	{
		// Token: 0x06000D1F RID: 3359 RVA: 0x0003AEDF File Offset: 0x000390DF
		private void Awake()
		{
			this.ikChain = base.GetComponent<IKSimpleChain>();
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0003AEED File Offset: 0x000390ED
		public void UpdateIKState(int targetState)
		{
			if (this.ikState != IKTargetPlant.IKState.Reset)
			{
				this.ikState = (IKTargetPlant.IKState)targetState;
			}
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x0003AEFF File Offset: 0x000390FF
		public Vector3 GetArcPosition(Vector3 start, Vector3 end, float arcHeight, float t)
		{
			return Vector3.Lerp(start, end, Mathf.Sin(t * 3.1415927f * 0.5f)) + new Vector3(0f, Mathf.Sin(t * 3.1415927f) * arcHeight, 0f);
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x0003AF40 File Offset: 0x00039140
		public void UpdateIKTargetPosition()
		{
			if (this.animator)
			{
				this.ikWeight = this.animator.GetFloat(this.animatorIKWeightFloat);
			}
			else
			{
				this.ikWeight = 1f;
			}
			IKTargetPlant.IKState ikstate = this.ikState;
			if (ikstate != IKTargetPlant.IKState.Plant)
			{
				if (ikstate == IKTargetPlant.IKState.Reset)
				{
					this.resetTimer += Time.deltaTime;
					this.isPlanted = false;
					this.RaycastIKTarget(base.transform.position);
					base.transform.position = this.GetArcPosition(this.plantPosition, this.targetPosition, this.arcHeight, this.resetTimer / this.timeToReset);
					if (this.resetTimer >= this.timeToReset)
					{
						this.ikState = IKTargetPlant.IKState.Plant;
						this.isPlanted = true;
						this.plantPosition = this.targetPosition;
						UnityEngine.Object.Instantiate<GameObject>(this.plantEffect, this.plantPosition, Quaternion.identity);
					}
				}
			}
			else
			{
				Vector3 position = base.transform.position;
				this.RaycastIKTarget(position);
				if (!this.isPlanted)
				{
					this.plantPosition = this.targetPosition;
					base.transform.position = this.plantPosition;
					this.isPlanted = true;
					if (this.plantEffect)
					{
						UnityEngine.Object.Instantiate<GameObject>(this.plantEffect, this.plantPosition, Quaternion.identity);
					}
				}
				else
				{
					base.transform.position = Vector3.Lerp(position, this.plantPosition, this.ikWeight);
				}
				Vector3 vector = position - base.transform.position;
				vector.y = 0f;
				if (this.ikChain.LegTooShort(this.legScale) || vector.sqrMagnitude >= this.maxXZPositionalError * this.maxXZPositionalError)
				{
					this.plantPosition = base.transform.position;
					this.ikState = IKTargetPlant.IKState.Reset;
					if (this.animator)
					{
						this.animator.SetTrigger(this.animatorLiftTrigger);
					}
					this.resetTimer = 0f;
				}
			}
			base.transform.position = Vector3.SmoothDamp(this.lastTransformPosition, base.transform.position, ref this.smoothDampRefVelocity, this.smoothDampTime);
			this.lastTransformPosition = base.transform.position;
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x0003B17C File Offset: 0x0003937C
		public void RaycastIKTarget(Vector3 position)
		{
			RaycastHit raycastHit;
			if (this.useSpherecast)
			{
				Physics.SphereCast(position + Vector3.up * -this.minHeight, this.spherecastRadius, Vector3.down, out raycastHit, this.maxHeight - this.minHeight, LayerIndex.world.mask);
			}
			else
			{
				Physics.Raycast(position + Vector3.up * -this.minHeight, Vector3.down, out raycastHit, this.maxHeight - this.minHeight, LayerIndex.world.mask);
			}
			if (raycastHit.collider)
			{
				this.targetPosition = raycastHit.point;
				return;
			}
			this.targetPosition = position;
		}

		// Token: 0x04000D3C RID: 3388
		[Tooltip("The max offset to step up")]
		public float minHeight = -0.3f;

		// Token: 0x04000D3D RID: 3389
		[Tooltip("The max offset to step down")]
		public float maxHeight = 1f;

		// Token: 0x04000D3E RID: 3390
		[Tooltip("The strength of the IK as a lerp (0-1)")]
		public float ikWeight = 1f;

		// Token: 0x04000D3F RID: 3391
		[Tooltip("The time to restep")]
		public float timeToReset = 0.6f;

		// Token: 0x04000D40 RID: 3392
		[Tooltip("The max positional IK error before restepping")]
		public float maxXZPositionalError = 4f;

		// Token: 0x04000D41 RID: 3393
		public GameObject plantEffect;

		// Token: 0x04000D42 RID: 3394
		public Animator animator;

		// Token: 0x04000D43 RID: 3395
		[Tooltip("The IK weight float parameter if used")]
		public string animatorIKWeightFloat;

		// Token: 0x04000D44 RID: 3396
		[Tooltip("The lift animation trigger string if used")]
		public string animatorLiftTrigger;

		// Token: 0x04000D45 RID: 3397
		[Tooltip("The scale of the leg for calculating if the leg is too short to reach the IK target")]
		public float legScale = 1f;

		// Token: 0x04000D46 RID: 3398
		[Tooltip("The height of the step arc")]
		public float arcHeight = 1f;

		// Token: 0x04000D47 RID: 3399
		[Tooltip("The smoothing duration for the IK. Higher will be smoother but will be delayed.")]
		public float smoothDampTime = 0.1f;

		// Token: 0x04000D48 RID: 3400
		[Tooltip("Spherecasts will have more hits but take higher performance.")]
		public bool useSpherecast;

		// Token: 0x04000D49 RID: 3401
		public float spherecastRadius = 0.5f;

		// Token: 0x04000D4A RID: 3402
		public IKTargetPlant.IKState ikState;

		// Token: 0x04000D4B RID: 3403
		private bool isPlanted;

		// Token: 0x04000D4C RID: 3404
		private Vector3 lastTransformPosition;

		// Token: 0x04000D4D RID: 3405
		private Vector3 smoothDampRefVelocity;

		// Token: 0x04000D4E RID: 3406
		private Vector3 targetPosition;

		// Token: 0x04000D4F RID: 3407
		private Vector3 plantPosition;

		// Token: 0x04000D50 RID: 3408
		private IKSimpleChain ikChain;

		// Token: 0x04000D51 RID: 3409
		private float resetTimer;

		// Token: 0x02000257 RID: 599
		public enum IKState
		{
			// Token: 0x04000D53 RID: 3411
			Plant,
			// Token: 0x04000D54 RID: 3412
			Reset
		}
	}
}
