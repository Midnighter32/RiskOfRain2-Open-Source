using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x0200056B RID: 1387
	public class SetCorrectiveLayer : StateMachineBehaviour
	{
		// Token: 0x06001EE3 RID: 7907 RVA: 0x00091C72 File Offset: 0x0008FE72
		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			base.OnStateMachineEnter(animator, stateMachinePathHash);
		}

		// Token: 0x06001EE4 RID: 7908 RVA: 0x00091C7C File Offset: 0x0008FE7C
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			int layerIndex2 = animator.GetLayerIndex(this.referenceOverrideLayerName);
			float target = Mathf.Min(animator.GetLayerWeight(layerIndex2), this.maxWeight);
			float weight = Mathf.SmoothDamp(animator.GetLayerWeight(layerIndex), target, ref this.smoothVelocity, 0.2f);
			animator.SetLayerWeight(layerIndex, weight);
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		// Token: 0x04002199 RID: 8601
		public string referenceOverrideLayerName;

		// Token: 0x0400219A RID: 8602
		public float maxWeight = 1f;

		// Token: 0x0400219B RID: 8603
		private float smoothVelocity;
	}
}
