using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000534 RID: 1332
	public class ResetFootsteps : StateMachineBehaviour
	{
		// Token: 0x06001F6F RID: 8047 RVA: 0x000887D8 File Offset: 0x000869D8
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.GetComponent<FootstepHandler>();
		}
	}
}
