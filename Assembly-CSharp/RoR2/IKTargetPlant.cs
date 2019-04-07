using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000334 RID: 820
	public class IKTargetPlant : MonoBehaviour, IIKTargetBehavior
	{
		// Token: 0x060010D2 RID: 4306 RVA: 0x00053F27 File Offset: 0x00052127
		private void Awake()
		{
			this.ikChain = base.GetComponent<IKSimpleChain>();
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x00053F35 File Offset: 0x00052135
		public void UpdateIKState(int targetState)
		{
			if (this.ikState != IKTargetPlant.IKState.Reset)
			{
				this.ikState = (IKTargetPlant.IKState)targetState;
			}
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x00053F47 File Offset: 0x00052147
		public Vector3 GetArcPosition(Vector3 start, Vector3 end, float arcHeight, float t)
		{
			return Vector3.Lerp(start, end, Mathf.Sin(t * 3.1415927f * 0.5f)) + new Vector3(0f, Mathf.Sin(t * 3.1415927f) * arcHeight, 0f);
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x00053F88 File Offset: 0x00052188
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

		// Token: 0x060010D6 RID: 4310 RVA: 0x000541C4 File Offset: 0x000523C4
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

		// Token: 0x040014F8 RID: 5368
		[Tooltip("The max offset to step up")]
		public float minHeight = -0.3f;

		// Token: 0x040014F9 RID: 5369
		[Tooltip("The max offset to step down")]
		public float maxHeight = 1f;

		// Token: 0x040014FA RID: 5370
		[Tooltip("The strength of the IK as a lerp (0-1)")]
		public float ikWeight = 1f;

		// Token: 0x040014FB RID: 5371
		[Tooltip("The time to restep")]
		public float timeToReset = 0.6f;

		// Token: 0x040014FC RID: 5372
		[Tooltip("The max positional IK error before restepping")]
		public float maxXZPositionalError = 4f;

		// Token: 0x040014FD RID: 5373
		public GameObject plantEffect;

		// Token: 0x040014FE RID: 5374
		public Animator animator;

		// Token: 0x040014FF RID: 5375
		[Tooltip("The IK weight float parameter if used")]
		public string animatorIKWeightFloat;

		// Token: 0x04001500 RID: 5376
		[Tooltip("The lift animation trigger string if used")]
		public string animatorLiftTrigger;

		// Token: 0x04001501 RID: 5377
		[Tooltip("The scale of the leg for calculating if the leg is too short to reach the IK target")]
		public float legScale = 1f;

		// Token: 0x04001502 RID: 5378
		[Tooltip("The height of the step arc")]
		public float arcHeight = 1f;

		// Token: 0x04001503 RID: 5379
		[Tooltip("The smoothing duration for the IK. Higher will be smoother but will be delayed.")]
		public float smoothDampTime = 0.1f;

		// Token: 0x04001504 RID: 5380
		[Tooltip("Spherecasts will have more hits but take higher performance.")]
		public bool useSpherecast;

		// Token: 0x04001505 RID: 5381
		public float spherecastRadius = 0.5f;

		// Token: 0x04001506 RID: 5382
		public IKTargetPlant.IKState ikState;

		// Token: 0x04001507 RID: 5383
		private bool isPlanted;

		// Token: 0x04001508 RID: 5384
		private Vector3 lastTransformPosition;

		// Token: 0x04001509 RID: 5385
		private Vector3 smoothDampRefVelocity;

		// Token: 0x0400150A RID: 5386
		private Vector3 targetPosition;

		// Token: 0x0400150B RID: 5387
		private Vector3 plantPosition;

		// Token: 0x0400150C RID: 5388
		private IKSimpleChain ikChain;

		// Token: 0x0400150D RID: 5389
		private float resetTimer;

		// Token: 0x02000335 RID: 821
		public enum IKState
		{
			// Token: 0x0400150F RID: 5391
			Plant,
			// Token: 0x04001510 RID: 5392
			Reset
		}
	}
}
