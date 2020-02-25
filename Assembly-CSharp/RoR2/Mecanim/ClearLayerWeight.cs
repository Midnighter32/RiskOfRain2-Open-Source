using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000530 RID: 1328
	public class ClearLayerWeight : StateMachineBehaviour
	{
		// Token: 0x06001F62 RID: 8034 RVA: 0x000884FC File Offset: 0x000866FC
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

		// Token: 0x06001F63 RID: 8035 RVA: 0x0008853C File Offset: 0x0008673C
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

		// Token: 0x04001D12 RID: 7442
		public string layerName;
	}
}
