using System;
using UnityEngine;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020008EB RID: 2283
	public class BeginBurrow : BaseState
	{
		// Token: 0x06003308 RID: 13064 RVA: 0x000DD378 File Offset: 0x000DB578
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			base.PlayCrossfade("Body", "BeginBurrow", "BeginBurrow.playbackRate", BeginBurrow.duration, 0.5f);
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
		}

		// Token: 0x06003309 RID: 13065 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600330A RID: 13066 RVA: 0x000DD3D4 File Offset: 0x000DB5D4
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

		// Token: 0x0600330B RID: 13067 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04003264 RID: 12900
		public static GameObject burrowPrefab;

		// Token: 0x04003265 RID: 12901
		public static float duration;

		// Token: 0x04003266 RID: 12902
		private bool isBurrowing;

		// Token: 0x04003267 RID: 12903
		private Animator animator;

		// Token: 0x04003268 RID: 12904
		private Transform modelTransform;

		// Token: 0x04003269 RID: 12905
		private ChildLocator childLocator;
	}
}
