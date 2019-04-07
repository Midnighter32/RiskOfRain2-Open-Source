using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000304 RID: 772
	[RequireComponent(typeof(ItemFollower))]
	public class GravCubeController : MonoBehaviour
	{
		// Token: 0x06000FE3 RID: 4067 RVA: 0x0004F921 File Offset: 0x0004DB21
		private void Start()
		{
			this.itemFollower = base.GetComponent<ItemFollower>();
			if (this.itemFollower)
			{
				this.itemFollowerAnimator = this.itemFollower.followerInstance.GetComponentInChildren<Animator>();
			}
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x0004F952 File Offset: 0x0004DB52
		public void ActivateCube(float duration)
		{
			this.activeTimer = duration;
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x0004F95C File Offset: 0x0004DB5C
		private void Update()
		{
			this.activeTimer -= Time.deltaTime;
			if (this.activeTimer > 0f)
			{
				this.itemFollowerAnimator.SetBool("active", true);
				return;
			}
			this.itemFollowerAnimator.SetBool("active", false);
		}

		// Token: 0x040013E7 RID: 5095
		private ItemFollower itemFollower;

		// Token: 0x040013E8 RID: 5096
		private float activeTimer;

		// Token: 0x040013E9 RID: 5097
		private Animator itemFollowerAnimator;
	}
}
