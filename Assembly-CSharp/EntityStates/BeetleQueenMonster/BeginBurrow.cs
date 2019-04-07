using System;
using UnityEngine;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020001D0 RID: 464
	internal class BeginBurrow : BaseState
	{
		// Token: 0x06000908 RID: 2312 RVA: 0x0002D710 File Offset: 0x0002B910
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			base.PlayCrossfade("Body", "BeginBurrow", "BeginBurrow.playbackRate", BeginBurrow.duration, 0.5f);
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x0002D76C File Offset: 0x0002B96C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = this.animator.GetFloat("BeginBurrow.active") > 0.9f;
			if (flag && !this.isBurrowing)
			{
				string childName = "BurrowCenter";
				Transform transform = this.childLocator.FindChild(childName);
				if (transform)
				{
					UnityEngine.Object.Instantiate<GameObject>(BeginBurrow.burrowPrefab, transform.position, Quaternion.identity);
				}
			}
			this.isBurrowing = flag;
			if (base.fixedAge >= BeginBurrow.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000C3C RID: 3132
		public static GameObject burrowPrefab;

		// Token: 0x04000C3D RID: 3133
		public static float duration;

		// Token: 0x04000C3E RID: 3134
		private bool isBurrowing;

		// Token: 0x04000C3F RID: 3135
		private Animator animator;

		// Token: 0x04000C40 RID: 3136
		private Transform modelTransform;

		// Token: 0x04000C41 RID: 3137
		private ChildLocator childLocator;
	}
}
