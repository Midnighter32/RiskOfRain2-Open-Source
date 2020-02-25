using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200021C RID: 540
	[RequireComponent(typeof(ItemFollower))]
	public class GravCubeController : MonoBehaviour
	{
		// Token: 0x06000BF0 RID: 3056 RVA: 0x0003573D File Offset: 0x0003393D
		private void Start()
		{
			this.itemFollower = base.GetComponent<ItemFollower>();
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x0003574B File Offset: 0x0003394B
		public void ActivateCube(float duration)
		{
			this.activeTimer = duration;
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x00035754 File Offset: 0x00033954
		private void Update()
		{
			if (this.itemFollower && this.itemFollower.followerInstance)
			{
				if (!this.itemFollowerAnimator)
				{
					this.itemFollowerAnimator = this.itemFollower.followerInstance.GetComponentInChildren<Animator>();
				}
				this.activeTimer -= Time.deltaTime;
				this.itemFollowerAnimator.SetBool("active", this.activeTimer > 0f);
			}
		}

		// Token: 0x04000BFD RID: 3069
		private ItemFollower itemFollower;

		// Token: 0x04000BFE RID: 3070
		private float activeTimer;

		// Token: 0x04000BFF RID: 3071
		private Animator itemFollowerAnimator;
	}
}
