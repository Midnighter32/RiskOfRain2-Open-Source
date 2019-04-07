using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000566 RID: 1382
	public class ClearLayerWeight : StateMachineBehaviour
	{
		// Token: 0x06001ED4 RID: 7892 RVA: 0x0009192C File Offset: 0x0008FB2C
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			int layerIndex2 = layerIndex;
			if (this.layerName.Length > 0)
			{
				layerIndex2 = animator.GetLayerIndex(this.layerName);
			}
			animator.SetLayerWeight(layerIndex2, 0f);
		}

		// Token: 0x06001ED5 RID: 7893 RVA: 0x0009196C File Offset: 0x0008FB6C
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			int layerIndex2 = layerIndex;
			if (this.layerName.Length > 0)
			{
				layerIndex2 = animator.GetLayerIndex(this.layerName);
			}
			animator.SetLayerWeight(layerIndex2, 1f);
		}

		// Token: 0x04002188 RID: 8584
		public string layerName;
	}
}
