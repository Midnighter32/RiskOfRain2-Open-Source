using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000535 RID: 1333
	public class SetCorrectiveLayer : StateMachineBehaviour
	{
		// Token: 0x06001F71 RID: 8049 RVA: 0x000887E6 File Offset: 0x000869E6
		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			base.OnStateMachineEnter(animator, stateMachinePathHash);
		}

		// Token: 0x06001F72 RID: 8050 RVA: 0x000887F0 File Offset: 0x000869F0
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			int layerIndex2 = animator.GetLayerIndex(this.referenceOverrideLayerName);
			float target = Mathf.Min(animator.GetLayerWeight(layerIndex2), this.maxWeight);
			float weight = Mathf.SmoothDamp(animator.GetLayerWeight(layerIndex), target, ref this.smoothVelocity, 0.2f);
			animator.SetLayerWeight(layerIndex, weight);
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		// Token: 0x04001D23 RID: 7459
		public string referenceOverrideLayerName;

		// Token: 0x04001D24 RID: 7460
		public float maxWeight = 1f;

		// Token: 0x04001D25 RID: 7461
		private float smoothVelocity;
	}
}
